using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

namespace Mandelbrot.UI {
  public class UISystem<T> : SystemBase
    where T : IComponentData {
    List<Action<EntityManager, T>> _preActions = new List<Action<EntityManager, T>>(100);
    List<Action<EntityManager, T>> _actions = new List<Action<EntityManager, T>>(100);
    EntityQuery _query;

    public void AddPreAction(Action<EntityManager, T> action) =>
      _preActions.Add(action);

    public void AddAction(Action<EntityManager, T> action) =>
      _actions.Add(action);

    protected override void OnCreate() {
      base.OnCreate();
      _query = GetEntityQuery(ComponentType.ReadOnly<T>());
    }

    protected override void OnUpdate() {
      var entities = _query.ToEntityArray(Allocator.Temp);
      InvokeActions(entities, _preActions);
      InvokeActions(entities, _actions);
    }

     void InvokeActions(NativeArray<Entity> entities, List<Action<EntityManager, T>> actions) {
      foreach (var a in actions)
        foreach (var entity in entities) {
          var uiData = EntityManager.GetComponentObject<T>(entity);
          a.Invoke(EntityManager, uiData);
        }
      actions.Clear();
    }
  }
}