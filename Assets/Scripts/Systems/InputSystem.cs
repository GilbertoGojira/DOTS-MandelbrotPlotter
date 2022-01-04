using Mandelbrot.Jobs;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Mandelbrot {
  [UpdateBefore(typeof(InputSystem))]
  [UpdateInGroup(typeof(InitializationSystemGroup))]
  public class ClearInputSystem : SystemBase {
    protected override void OnUpdate() {
      Dependency = Entities
        .WithChangeFilter<InputStatus>()
         .ForEach((ref InputStatus status) =>
            status.Type = InputType.NONE)
        .ScheduleParallel(Dependency);
    }
  }

  [UpdateInGroup(typeof(InitializationSystemGroup))]
  public class InputSystem : SystemBase {
    BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
    EntityQuery _query;
    EntityQuery _missingQuery;

    protected override void OnCreate() {
      base.OnCreate();
      _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
      _query = GetEntityQuery(ComponentType.ReadOnly<ComponentCopy<Viewport>>(), ComponentType.ReadWrite<Viewport>());
      _missingQuery = GetEntityQuery(ComponentType.ReadOnly<ScreenRenderBounds>(), ComponentType.Exclude<InputStatus>());
    }

    protected override void OnUpdate() {
      EntityManager.AddComponent(_missingQuery, typeof(InputStatus));

      var leftMouseDown = Input.GetMouseButton(0);
      var rightMouseDown = Input.GetMouseButton(1);
      var mousePosition = Input.mousePosition;

      if (leftMouseDown || rightMouseDown)
        Dependency = Entities
          .ForEach((ref InputStatus status, in ScreenRenderBounds bounds) => {
            if (!bounds.Value.Contains(mousePosition))
              return;
            status.Type = leftMouseDown ? InputType.LEFT_CLICK : status.Type;
            status.Type = rightMouseDown ? InputType.RIGHT_CLICK : status.Type;
            status.Position = new float2(mousePosition.x, mousePosition.y);
          }).ScheduleParallel(Dependency);

      if (Input.GetKeyDown(KeyCode.R))
        Dependency = new RestoreComponentDataCopy<ComponentCopy<Viewport>, Viewport> {
          ECB = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter(),
          EntityHandle = GetEntityTypeHandle(),
          ComponentCopyTypeHandle = GetComponentTypeHandle<ComponentCopy<Viewport>>(true)
        }.ScheduleParallel(_query, Dependency);


      _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
  }
}
