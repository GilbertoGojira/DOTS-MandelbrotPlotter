using System;
using Unity.Entities;

namespace Mandelbrot {
  [Serializable]
  public struct Iterations : IComponentData {
    public int Value;
  }
}
