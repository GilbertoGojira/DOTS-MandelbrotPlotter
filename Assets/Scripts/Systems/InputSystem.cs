using Mandelbrot.Jobs;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Mandelbrot {
  public class InputSystem : SystemBase {
    BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
    EntityQuery _query;

    protected override void OnCreate() {
      base.OnCreate();
      _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
      _query = GetEntityQuery(ComponentType.ReadOnly<ComponentCopy<Viewport>>(), ComponentType.ReadWrite<Viewport>());
    }

    protected override void OnUpdate() {
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
