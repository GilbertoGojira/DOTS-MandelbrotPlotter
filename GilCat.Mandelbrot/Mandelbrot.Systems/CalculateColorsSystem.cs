using System.Collections.Generic;
using System.Diagnostics;
using Mandelbrot.Collections;
using Mandelbrot.Components;
using Mandelbrot.Jobs;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Resolution = Mandelbrot.Components.Resolution;

namespace Mandelbrot.Systems {
  public class CalculateColorsSystem : SystemBase {
    EntityQuery _query;
    EntityQuery _missingQuery;
    NativeList<JobHandle> _jobHandles;
    // One counter per mandelbrot 
    List<(Entity, NativeCounter, Stopwatch)> _statsCounters = new List<(Entity, NativeCounter, Stopwatch)>();

    void ClearStatsCounters() {
      for (var i = 0; i < _statsCounters.Count; ++i) {
        var c = _statsCounters[i];
        c.Item2.Value = 0;
        _statsCounters[i] = c;
      }
    }

    void ResizeStatsCounters(int size) {
      for (var i = size; i < _statsCounters.Count; ++i)
        _statsCounters[i].Item2.Dispose();
      _statsCounters.RemoveRange(size, _statsCounters.Count - size);
    }

    void DisposeStatsConters() =>
      ResizeStatsCounters(0);

    (Entity, NativeCounter, Stopwatch) GetStatsCounter(int index, Entity entity) {
      if (index >= _statsCounters.Count)
        _statsCounters.Add((entity, new NativeCounter(Allocator.Persistent), new Stopwatch()));
      else
        _statsCounters[index] = (entity, _statsCounters[index].Item2, _statsCounters[index].Item3);
      return _statsCounters[index];
    }

    protected override void OnCreate() {
      base.OnCreate();
      _missingQuery = GetEntityQuery(typeof(HSVColor), ComponentType.Exclude<PointColor>());
      _jobHandles = new NativeList<JobHandle>(Allocator.Persistent);
    }

    protected override void OnDestroy() {
      base.OnDestroy();
      _jobHandles.Dispose();
      DisposeStatsConters();
    }

    protected override void OnUpdate() {
      EntityManager.AddComponent(_missingQuery,
        new ComponentTypes(ComponentType.ReadWrite<PointColor>(), ComponentType.ReadWrite<Stats>()));

      // Only calculate if there are entities that were changed
      // _query is bound to change filter for Config and TextureConfig
      // this means if any of those are touched the query will yield the number of changed entities
      // NOTE: _query will be processed at comile time and will be expanded into:
      // GetEntityQuery(PointColor, Config, TextureConfig) with changed filter
      if (_query.CalculateEntityCount() == 0)
        return;
      _jobHandles.Clear();
      ClearStatsCounters();
      var counterIndex = 0;
      Entities
        .WithoutBurst()
        .WithStoreEntityQueryInField(ref _query)
        .WithChangeFilter<Config, Resolution>()
        .ForEach((Entity entity, DynamicBuffer<PointColor> colors, in Config config, in Resolution resolution) => {
          if (!config.CalculationReady)
            return;
          var (_, counter, watch) = GetStatsCounter(counterIndex, entity);
          watch.Reset();
          watch.Start();
          var width = resolution.Width;
          var height = resolution.Height;
          colors.ResizeUninitialized(width * height);
          _jobHandles.Add(new GenerateColorJob {
            Colors = colors.Reinterpret<Color32>().AsNativeArray(),
            Config = config,
            Width = width,
            Height = height,
            Step = new double2(config.Viewport.Width / width, config.Viewport.Height / height),
            TotalIterations = counter
          }.ScheduleParallel(width, 128, Dependency));
          counterIndex++;
      }).Run();
      ResizeStatsCounters(counterIndex);

#if STATS_ENABLED
      // When we are profiling stats we complete each job separately
      // This will give us a reliable measure for each job duration
      // When not profiling we try to parellelize all the jobs as much as possible
      // ans that wil give an overall better result when many jobs
      counterIndex = 0;
      foreach (var (entity, counter, watch) in _statsCounters) {
        _jobHandles[counterIndex++].Complete();
        watch.Stop();
        EntityManager.SetComponentData(entity, new Stats {
          Iterations = counter.Value,
          Duration = watch.ElapsedMilliseconds
        });
      }
#endif
      Dependency = JobHandle.CombineDependencies(_jobHandles);
    }
  }
}
