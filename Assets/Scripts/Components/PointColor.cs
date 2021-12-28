using Unity.Entities;
using UnityEngine;

namespace Mandelbrot {
  public struct PointColor : IBufferElementData {
    public Color32 Value;
  }
}