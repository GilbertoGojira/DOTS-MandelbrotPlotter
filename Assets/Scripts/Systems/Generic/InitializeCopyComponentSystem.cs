using Mandelbrot.Jobs;
using Unity.Entities;
using Unity.Jobs;

/// <summary>
/// Will create a copy of the component when that concrete component occurs
/// The component will be cloned into a ComponentCopy<[Original Component]>
/// </summary>
namespace Mandelbrot {
  public class InitializeCopyComponentSystem<T> : SystemBase
    where T : struct, IComponentData {
    BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;
    EntityQuery _missingQuery;

    protected override void OnCreate() {
      base.OnCreate();
      _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
      _missingQuery = GetEntityQuery(ComponentType.ReadOnly<T>(), ComponentType.Exclude<ComponentCopy<T>>());
    }

    protected override void OnUpdate() {
      Dependency = new ComponentCopyJob<ComponentCopy<T>, T> {
        ECB = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter(),
        EntityHandle = GetEntityTypeHandle(),
        ComponentTypeHandle = GetComponentTypeHandle<T>(true)
      }.ScheduleParallel(_missingQuery, Dependency);
      _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
  }
}
