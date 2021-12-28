using Mandelbrot;
using Mandelbrot.Jobs;
using Unity.Entities;
using Unity.Jobs;

[assembly: RegisterGenericComponentType(typeof(ComponentCopy<Config>))]
namespace Mandelbrot {
  public class InitializeCopyComponent<T> : SystemBase
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
