using System;
using Unity.Entities;

namespace Mandelbrot.Components {
  [Serializable]
  public struct Resolution : IComponentData {
    public int Width;
    public int Height;
  }
}