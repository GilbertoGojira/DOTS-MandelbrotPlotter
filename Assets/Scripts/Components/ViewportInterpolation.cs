using Unity.Entities;
using Unity.Mathematics;

namespace Mandelbrot {
  public struct ViewportInterpolation : IComponentData {
    public Viewport Source;
    public Viewport Target;
  }

  public struct InterpolationTime : IComponentData {
    public float Elapsed;
    public float Duration;
  }

  /// <summary>
  /// Normalized animation curve
  /// </summary>
  public struct AnimationCurve : IBufferElementData {
    public float Value;

    public static implicit operator AnimationCurve(float value) =>
      new AnimationCurve { Value = value };
  }

  public static class AnimationCurveExt {
    public static float Evalute(this DynamicBuffer<AnimationCurve> curve, float time) {
      return curve[(int)math.round((curve.Length - 1) * time)].Value;
    }
  }
}
