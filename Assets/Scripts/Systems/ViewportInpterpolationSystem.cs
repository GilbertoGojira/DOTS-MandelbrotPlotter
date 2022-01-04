using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Mandelbrot {

  /// <summary>
  /// Interpolates viewport between 2 values during a given time
  /// </summary>
  public class ViewportInpterpolationSystem : SystemBase {
    BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;

    protected override void OnCreate() {
      base.OnCreate();
      _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate() {
      var dt = Time.DeltaTime;
      Dependency = Entities
        .ForEach((ref InterpolationTime time) => {
          time.Elapsed += dt / time.Duration;
          time.Elapsed = math.min(time.Elapsed, 1f);
        }).ScheduleParallel(Dependency);

      Dependency = Entities
        .ForEach((ref Viewport viewport, in ViewportInterpolation interpolation, in InterpolationTime time) => {
          var source = interpolation.Source;
          var target = interpolation.Target;
          var min = math.lerp(source.Min, target.Min, time.Elapsed);
          var max = math.lerp(source.Max, target.Max, time.Elapsed);
          viewport = new MinMaxAABB { Min = min, Max = max };
        }).ScheduleParallel(Dependency);

      var ecb = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
      Dependency = Entities
        .ForEach((Entity entity, int entityInQueryIndex, in InterpolationTime time) => {
          if(time.Elapsed >= 1f) {
            ecb.RemoveComponent<InterpolationTime>(entityInQueryIndex, entity);
            ecb.RemoveComponent<ViewportInterpolation>(entityInQueryIndex, entity);
          }
        }).ScheduleParallel(Dependency);
      _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
  }
}
