using System.Collections.Generic;
using Mandelbrot.Collections;
using Mandelbrot.Jobs;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Mandelbrot {

  public class CalculateColors : SystemBase {
    BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
    EntityQuery _query;
    EntityQuery _missingQuery;
    NativeList<JobHandle> _jobHandles;
    // One counter per mandelbrot 
    List<(Entity, NativeCounter)> _iterationCounters = new List<(Entity, NativeCounter)>();

    void ClearCounters() {
      for (var i = 0; i < _iterationCounters.Count; ++i) {
        var c = _iterationCounters[i];
        c.Item2.Value = 0;
        _iterationCounters[i] = c;
      }
    }

    void ResizeCounters(int size) {
      for (var i = size; i < _iterationCounters.Count; ++i)
        _iterationCounters[i].Item2.Dispose();
      _iterationCounters.RemoveRange(size, _iterationCounters.Count - size);
    }

    NativeCounter GetCounter(int index, Entity entity) {
      if (index >= _iterationCounters.Count)
        _iterationCounters.Add((entity, new NativeCounter(Allocator.Persistent)));
      else
        _iterationCounters[index] = (entity, _iterationCounters[index].Item2);
      return _iterationCounters[index].Item2;
    }

    protected override void OnCreate() {
      base.OnCreate();
      _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
      _missingQuery = GetEntityQuery(typeof(Config), ComponentType.Exclude<PointColor>());
      _jobHandles = new NativeList<JobHandle>(Allocator.Persistent);
    }

    protected override void OnDestroy() {
      base.OnDestroy();
      _jobHandles.Dispose();
      ResizeCounters(0);
    }

    protected override void OnUpdate() {
      EntityManager.AddComponent(_missingQuery,
        new ComponentTypes(ComponentType.ReadWrite<PointColor>(), ComponentType.ReadWrite<Stats>()));

      if (_query.CalculateEntityCount() == 0)
        return;
      _jobHandles.Clear();
      ClearCounters();
      var counterIndex = 0;
      Entities
        .WithoutBurst()
        .WithStoreEntityQueryInField(ref _query)
        .WithChangeFilter<Config, TextureConfig>()
        .ForEach((Entity entity, DynamicBuffer<PointColor> colors, in Config config, in TextureConfig textureConfig) => {
          var width = textureConfig.Width;
          var height = textureConfig.Height;
          colors.ResizeUninitialized(width * height);
          _jobHandles.Add(new GenerateColorJob {
            Colors = colors.Reinterpret<Color32>().AsNativeArray(),
            Config = config,
            Width = width,
            Height = height,
            Step = new double2(config.Viewport.Width / width, config.Viewport.Height / height),
            TotalIterations = GetCounter(counterIndex, entity)
          }.ScheduleParallel(width, 128, Dependency));
          counterIndex++;
      }).Run();
      ResizeCounters(counterIndex);
      counterIndex = 0;
      Dependency = JobHandle.CombineDependencies(_jobHandles);

      // Copy gathered stats into the component
      var ecb = _entityCommandBufferSystem.CreateCommandBuffer();
      foreach(var (entity, counter) in _iterationCounters)
        Dependency = JobHandle.CombineDependencies(
          Job
          .WithNativeDisableContainerSafetyRestriction(counter)
          .WithCode(() =>
          ecb.SetComponent(entity, new Stats { Value = counter.Value }))
        .Schedule(Dependency), Dependency);

      _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
  }
}
