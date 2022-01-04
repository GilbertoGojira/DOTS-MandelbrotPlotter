using Unity.Entities;

namespace Mandelbrot {
  public struct ViewportInterpolation : IComponentData {
    public Viewport Source;
    public Viewport Target;
    // TODO: create a blitable version for curve
    //public AnimationCurve Curve;
  }

  public struct InterpolationTime : IComponentData {
    public float Elapsed;
    public float Duration;
  }
}
