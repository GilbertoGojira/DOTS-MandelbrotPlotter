using Unity.Entities;

namespace Mandelbrot.Components {
  public struct InterpolationTime : IComponentData {
    public float Elapsed;
    public float Duration;
  }
}
