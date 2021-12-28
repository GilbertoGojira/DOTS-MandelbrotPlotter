using System;
using Unity.Entities;

namespace Mandelbrot {
  public interface IComponentDataCopy<T> : IComponentData
    where T : struct, IComponentData {
    T Value { get; set; }
  }

  [Serializable]
  public struct ComponentCopy<T> : IComponentDataCopy<T>
    where T : struct, IComponentData {
    public T Value { get; set; }
  }
}