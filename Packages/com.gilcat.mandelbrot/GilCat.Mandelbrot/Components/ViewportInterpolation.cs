using Unity.Entities;

namespace Mandelbrot.Components {
  public struct ViewportInterpolation : IComponentData {
    public Viewport Source;
    public Viewport Target;
  }
}