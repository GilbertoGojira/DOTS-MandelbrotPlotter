using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Mandelbrot {

  public class ZoomSystem : SystemBase {
    BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;

    protected override void OnCreate() {
      base.OnCreate();
      _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate() {
      var dt = Time.DeltaTime;
      Entities
        .ForEach((ref ZoomTime zoomTime, in ZoomViewport zoom) => {
          zoomTime.Value += dt / zoom.Duration;
          zoomTime.Value = math.max(zoomTime.Value, 1f);
        }).ScheduleParallel(Dependency);

      Entities
        .ForEach((ref Viewport viewport, in ZoomViewport zoom, in ZoomTime zoomTime) => {
          var source = zoom.Source;
          var target = zoom.Target;
          var min = math.lerp(source.Min, target.Min, zoomTime.Value);
          var max = math.lerp(source.Max, target.Max, zoomTime.Value);
          viewport = new Viewport { Min = min, Max = max };
        }).ScheduleParallel(Dependency);

      var ecb = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
      Entities
        .ForEach((Entity entity, int entityInQueryIndex, in ZoomTime zoomTime) => {
          if(zoomTime.Value >= 1f) {
            ecb.RemoveComponent<ZoomTime>(entityInQueryIndex, entity);
            ecb.RemoveComponent<ZoomViewport>(entityInQueryIndex, entity);
          }
        }).ScheduleParallel(Dependency);
      _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }

    public void StartZoom(Entity entity, Camera camera, float factor, Viewport source, float2 targetPoint, float duration) {
      var target = default(Viewport); //Utilities.GetScreenPointInsideViewport(camera, this, source, targetPoint);
      EntityManager.AddComponent<ZoomTime>(entity);
      EntityManager.AddComponentData(entity, new ZoomViewport {
        Source = source,
        Target = target,
        Duration = duration
      });
    }
  }
}
