using Unity.Entities;
using UnityEngine;

namespace Mandelbrot.Components {
  public struct PointColor : IBufferElementData {
    public Color32 Value;
  }

  public struct Index : IBufferElementData {
    public int Value;
  }
}