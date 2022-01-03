using Unity.Entities;

namespace Mandelbrot {
  public struct ZoomViewport : IComponentData {
    public Viewport Source;
    public Viewport Target;
    public float Duration;
    // TODO: create a blitable version for curve
    //public AnimationCurve Curve;
  }

  public struct ZoomTime : IComponentData {
    public float Value;
  }
}
