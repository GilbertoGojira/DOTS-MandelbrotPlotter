using Unity.Entities;
using Unity.Mathematics;

namespace Mandelbrot {
  public struct ScreenRenderBounds : IComponentData {
    public AABB Value;
  }
}
