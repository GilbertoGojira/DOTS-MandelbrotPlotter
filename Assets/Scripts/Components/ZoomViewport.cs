using Unity.Entities;

namespace Mandelbrot {
  public struct ZoomViewport : IComponentData {
    public Viewport Source;
    public Viewport Target;
    // TODO: create a blitable version for curve
    //public AnimationCurve Curve;
  }

  public struct ZoomTime : IComponentData {
    public float Elapsed;
    public float Duration;
  }
}
