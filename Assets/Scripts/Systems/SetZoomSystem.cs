using Unity.Entities;
using Unity.Mathematics;

namespace Mandelbrot {
  public class SetZoomSystem : SystemBase {
    BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;

    protected override void OnCreate() {
      base.OnCreate();
      _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate() {
      var ecb = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
      Dependency = Entities
        .WithChangeFilter<InputStatus>()
        .ForEach((Entity entity, int entityInQueryIndex, in ScreenRenderBounds bounds, in Viewport viewport, in InputStatus status, in MandelbrotConfig config) => {
          if(status.Type != InputType.NONE)
            StartZoom(ecb, entityInQueryIndex, entity, bounds.Value, viewport, GetZoomFactor(status.Type, config), status.Position, 2f);
        }).ScheduleParallel(Dependency);
      _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }

    static void StartZoom(EntityCommandBuffer.ParallelWriter ecb, int sortKey, Entity entity, AABB renderBounds, Viewport source, float factor, float2 targetPoint, float duration) {
      Viewport target = Utilities.GetScreenPointInsideViewport(renderBounds, source, targetPoint);
      target.Width = source.Width * factor;
      target.Height = source.Height * factor;
      ecb.AddComponent(sortKey, entity, new ZoomTime { Duration = duration });
      ecb.AddComponent(sortKey, entity, new ZoomViewport {
        Source = source,
        Target = target
      });
    }

    static float GetZoomFactor(InputType type, MandelbrotConfig config) {
      switch (type) {
        case InputType.LEFT_CLICK:
          return config.ZoomInFactor;
        case InputType.RIGHT_CLICK:
          return config.ZoomOutFactor;
      }
      return 0;
    }
  }
}
