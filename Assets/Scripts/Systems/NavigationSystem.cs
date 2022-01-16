using Mandelbrot.Jobs;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Mandelbrot.Components;

namespace Mandelbrot {
  public class NavigationSystem : SystemBase {
    BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
    EntityQuery _query;
    EntityQuery _zoomQuery;

    protected override void OnCreate() {
      base.OnCreate();
      _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
      _query = GetEntityQuery(ComponentType.ReadOnly<ComponentCopy<Viewport>>(), ComponentType.ReadWrite<Viewport>());
      _zoomQuery = GetEntityQuery(typeof(InterpolationTime), typeof(ViewportInterpolation));
    }

    protected override void OnUpdate() {
      var ecb = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
      Dependency = Entities
        .WithChangeFilter<InputStatus>()
        .ForEach((Entity entity, int entityInQueryIndex, ref Config config, in Zoom zoom, in ScreenRenderBounds bounds, in Viewport viewport, in InputStatus status) => {
          if (status.Type != InputType.NONE) {
            if (!config.CalculationReady)
              config.CalculationReady = true;
            else
              StartZoom(ecb, entityInQueryIndex, entity, bounds.Value, viewport, GetZoomFactor(status.Type, zoom), status.Position, zoom.Duration);
          }
        }).ScheduleParallel(Dependency);

      // TODO: Extend input system for keys
      if (Input.GetKeyDown(KeyCode.R)) {
        EntityManager.RemoveComponent(_zoomQuery, new ComponentTypes(typeof(InterpolationTime), typeof(ViewportInterpolation)));
        Dependency = new RestoreComponentDataCopy<ComponentCopy<Viewport>, Viewport> {
          ECB = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter(),
          EntityHandle = GetEntityTypeHandle(),
          ComponentCopyTypeHandle = GetComponentTypeHandle<ComponentCopy<Viewport>>(true)
        }.ScheduleParallel(_query, Dependency);
      }

      _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }

    static void StartZoom(EntityCommandBuffer.ParallelWriter ecb, int sortKey, Entity entity, AABB renderBounds, Viewport source, float factor, float2 targetPoint, float duration) {
      Viewport target = Utilities.GetScreenPointInsideViewport(renderBounds, source.Value, targetPoint);
      target.Width = source.Width * factor;
      target.Height = source.Height * factor;
      ecb.AddComponent(sortKey, entity, new InterpolationTime { Duration = duration });
      ecb.AddComponent(sortKey, entity, new ViewportInterpolation {
        Source = source,
        Target = target
      });
    }

    static float GetZoomFactor(InputType type, Zoom config) {
      switch (type) {
        case InputType.LEFT_CLICK:
          return config.InFactor;
        case InputType.RIGHT_CLICK:
          return config.OutFactor;
      }
      return 0;
    }
  }
}
