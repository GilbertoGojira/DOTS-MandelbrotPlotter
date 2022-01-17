using Mandelbrot.Components;
using Unity.Entities;
using Unity.Jobs;

namespace Mandelbrot.Systems {
  [UpdateBefore(typeof(CalculateColorsSystem))]
  public class CopyToConfigSystem : SystemBase {
    BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;

    protected override void OnCreate() {
      base.OnCreate();
      _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate() {
      var ecb = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
      Dependency = Entities
        .WithNone<Viewport, HSVColor, Iterations>()
        .ForEach((Entity entity, int entityInQueryIndex, in Config config) => {
          ecb.AddComponent(entityInQueryIndex, entity, config.Resolution);
          ecb.AddComponent(entityInQueryIndex, entity, config.Viewport);
          ecb.AddComponent(entityInQueryIndex, entity, config.ColorSetup);
          ecb.AddComponent(entityInQueryIndex, entity, config.Iterations);
          ecb.AddComponent(entityInQueryIndex, entity, config.Zoom);
        }).ScheduleParallel(Dependency);

      Dependency = Entities
        .WithChangeFilter<Viewport>()
        .ForEach((ref Config config, in Viewport viewport) => {
          config.Viewport = viewport;
        }).ScheduleParallel(Dependency);

      Dependency = Entities
        .WithChangeFilter<HSVColor>()
        .ForEach((ref Config config, in HSVColor colorSetup) => {
          config.ColorSetup = colorSetup;
        }).ScheduleParallel(Dependency);

      Dependency = Entities
        .WithChangeFilter<Iterations>()
        .ForEach((ref Config config, in Iterations iterations) => {
          config.Iterations = iterations;
        }).ScheduleParallel(Dependency);
      _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
  }
}
