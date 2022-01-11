using Unity.Collections;
using Unity.Entities;

namespace Mandelbrot.UI {

  public class UISystem<T> : ComponentSystem
    where T : struct, IComponentData {
    EntityQuery _setQuery;
    EntityQuery _getQuery;

    protected override void OnCreate() {
      base.OnCreate();
      _getQuery = GetEntityQuery(ComponentType.ReadOnly<UIGetAction<T>>(), ComponentType.ReadOnly<T>());
      _setQuery = GetEntityQuery(ComponentType.ReadOnly<UISetAction<T>>());
    }

    protected override void OnUpdate() {
      var entityType = GetEntityTypeHandle();

      var getTypeHandle = EntityManager.GetComponentTypeHandle<UIGetAction<T>>(true);
      var compTypeHandle= GetComponentTypeHandle<T>(true);

      _getQuery.SetChangedVersionFilter(typeof(T));
      var chunks = _getQuery.CreateArchetypeChunkArray(Allocator.Temp);
      foreach (var chunk in chunks) {
        var actions = chunk.GetManagedComponentAccessor(getTypeHandle, EntityManager);
        var comps = chunk.GetNativeArray(compTypeHandle);
        for (var i = 0; i < actions.Length; ++i)
          actions[i].Invoke(comps[i]);
      }

      var setTypeHandle = EntityManager.GetComponentTypeHandle<UISetAction<T>>(true);
      var compFromEntity = GetComponentDataFromEntity<T>();

      chunks = _setQuery.CreateArchetypeChunkArray(Allocator.Temp);
      foreach(var chunk in chunks) {
        var entities = chunk.GetNativeArray(entityType);
        var actions = chunk.GetManagedComponentAccessor(setTypeHandle, EntityManager);
        for (var i = 0; i < entities.Length; ++i) {
          var c = compFromEntity[actions[i].Target];
          actions[i].Invoke(ref c);
          compFromEntity[actions[i].Target] = c;
        }
      }

      EntityManager.DestroyEntity(_setQuery);
    }
  }

  public class UISystem<T1, T2> : ComponentSystem
    where T1 : struct, IComponentData
    where T2 : struct, IComponentData {

    EntityQuery _getQuery;

    protected override void OnCreate() {
      base.OnCreate();
      _getQuery = GetEntityQuery(
        ComponentType.ReadOnly<UIGetAction<T1, T2>>(),
        ComponentType.ReadOnly<T1>(),
        ComponentType.ReadOnly<T2>());
    }

    protected override void OnUpdate() {
      var getTypeHandle = EntityManager.GetComponentTypeHandle<UIGetAction<T1, T2>>(true);
      var comp1TypeHandle = GetComponentTypeHandle<T1>(true);
      var comp2TypeHandle = GetComponentTypeHandle<T2>(true);

      _getQuery.SetChangedVersionFilter(typeof(T1));
      var chunks = _getQuery.CreateArchetypeChunkArray(Allocator.Temp);
      foreach (var chunk in chunks) {
        var actions = chunk.GetManagedComponentAccessor(getTypeHandle, EntityManager);
        var comps1 = chunk.GetNativeArray(comp1TypeHandle);
        var comps2 = chunk.GetNativeArray(comp2TypeHandle);
        for (var i = 0; i < actions.Length; ++i)
          actions[i].Invoke(comps1[i], comps2[i]);
      }
    }
  }

  public class UISystem<T1, T2, T3> : ComponentSystem
    where T1 : struct, IComponentData
    where T2 : struct, IComponentData
    where T3 : struct, IComponentData {

    EntityQuery _getQuery;

    protected override void OnCreate() {
      base.OnCreate();
      _getQuery = GetEntityQuery(
        ComponentType.ReadOnly<UIGetAction<T1, T2, T3>>(),
        ComponentType.ReadOnly<T1>(),
        ComponentType.ReadOnly<T2>(),
        ComponentType.ReadOnly<T3>());
    }

    protected override void OnUpdate() {
      var getTypeHandle = EntityManager.GetComponentTypeHandle<UIGetAction<T1, T2, T3>>(true);
      var comp1TypeHandle = GetComponentTypeHandle<T1>(true);
      var comp2TypeHandle = GetComponentTypeHandle<T2>(true);
      var comp3TypeHandle = GetComponentTypeHandle<T3>(true);

      _getQuery.SetChangedVersionFilter(typeof(T1));
      var chunks = _getQuery.CreateArchetypeChunkArray(Allocator.Temp);
      foreach (var chunk in chunks) {
        var actions = chunk.GetManagedComponentAccessor(getTypeHandle, EntityManager);
        var comps1 = chunk.GetNativeArray(comp1TypeHandle);
        var comps2 = chunk.GetNativeArray(comp2TypeHandle);
        var comps3 = chunk.GetNativeArray(comp3TypeHandle);
        for (var i = 0; i < actions.Length; ++i)
          actions[i].Invoke(comps1[i], comps2[i], comps3[i]);
      }
    }
  }
}
