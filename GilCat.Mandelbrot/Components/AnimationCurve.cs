using Unity.Entities;
using Unity.Mathematics;
using Mandelbrot.Components;

namespace Mandelbrot.Components {
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
