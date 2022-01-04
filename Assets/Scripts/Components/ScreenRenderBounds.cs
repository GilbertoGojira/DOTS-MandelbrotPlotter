using Unity.Entities;
using Unity.Mathematics;

namespace Mandelbrot {
  public struct ScreenRenderBounds : IComponentData {
    public AABB Value;

    public float Width {
      get => Value.Size.x;
    }

    public float Height {
      get => Value.Size.y;
    }
  }
}
