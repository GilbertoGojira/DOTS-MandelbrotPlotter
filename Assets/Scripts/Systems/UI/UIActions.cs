using System;
using Unity.Entities;

namespace Mandelbrot.UI {

  public delegate void ActionRef<T>(ref T item);

  public class UISetAction<T> : IComponentData {
    public Entity Target;
    public ActionRef<T> Action;

    public void Invoke(ref T item) =>
      Action.Invoke(ref item);
  }

  public class UIGetAction<T> : IComponentData {
    public Entity Source;
    public Action<Entity, T> Action;

    public void Invoke(T item) =>
      Action.Invoke(Source, item);
  }

  public class UIGetAction<T1, T2> : IComponentData {
    public Entity Source;
    public Action<Entity, T1, T2> Action;

    public void Invoke(T1 item1, T2 item2) =>
      Action.Invoke(Source, item1, item2);
  }

  public class UIGetAction<T1, T2, T3> : IComponentData {
    public Entity Source;
    public Action<Entity, T1, T2, T3> Action;

    public void Invoke(T1 item1, T2 item2, T3 item3) =>
      Action.Invoke(Source, item1, item2, item3);
  }

  /// <summary>
  /// utilities to help create UI Actions
  /// </summary>
  public static class Utilities {
    public static void CreateUIGetAction<T>(EntityManager entityManager, Entity source, Action<T> getAction, Entity target) {
      entityManager.AddComponentObject(target, new UIGetAction<T> {
        Source = source,
        Action = (e, item) => {
          if (e == source) getAction(item);
        }
      });
    }

    public static void CreateUIGetAction<T>(EntityManager entityManager, Entity source, Action<Entity, T> getAction, Entity target) {
      entityManager.AddComponentObject(target, new UIGetAction<T> {
        Source = source,
        Action = (e, item) => {
          if (e == source) getAction(e, item);
        }
      });
    }

    public static void CreateUIGetAction<T1, T2, T3>(EntityManager entityManager, Entity source, Action<T1, T2, T3> getAction, Entity target) {
      entityManager.AddComponentObject(target, new UIGetAction<T1, T2, T3> {
        Source = source,
        Action = (e, item1,item2, item3) => {
          if (e == source) getAction(item1, item2, item3);
        }
      });
    }

    public static void CreateUIGetAction<T1, T2, T3>(EntityManager entityManager, Entity source, Action<Entity, T1, T2, T3> getAction, Entity target) {
      entityManager.AddComponentObject(target, new UIGetAction<T1, T2, T3> {
        Source = source,
        Action = (e, item1, item2, item3) => {
          if (e == source) getAction(e, item1, item2, item3);
        }
      });
    }

    public static void CreateUISetAction<T>(EntityManager entityManager, Entity target, ActionRef<T> setAction) {
      entityManager
        .AddComponentObject(entityManager.CreateEntity(), new UISetAction<T> {
          Target = target,
          Action = setAction
        });
    }
  }
}
