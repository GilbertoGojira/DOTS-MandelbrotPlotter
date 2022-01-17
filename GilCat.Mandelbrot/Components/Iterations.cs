using System;
using Unity.Entities;

namespace Mandelbrot.Components {
  [Serializable]
  public struct Iterations : IComponentData {
    public int Value;
  }
}
