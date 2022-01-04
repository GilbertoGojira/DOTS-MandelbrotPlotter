using Unity.Mathematics;
using UnityEngine;

namespace Mandelbrot {
  public static class Utilities {
    /// <summary>
    /// Gets a color from HSV values and powers
    /// </summary>
    /// <param name="value">A normalized value for one of the H, S or V</param>
    /// <param name="hsvIndex">The index for HSV member (0 - H, 1 - S, 2 - V)</param>
    /// <param name="hsv">The HSV value</param>
    /// <param name="powers">The powers to raise the HSV values</param>
    /// <param name="defaultColor">The default to return when the normalized value is 1</param>
    /// <returns>The RGB color computed rom the HSV values</returns>
    public static Color32 GetColor(float value, int hsvIndex, float3 hsv, float3 powers, Color32 defaultColor) {
      Color32 color = Color.HSVToRGB(
        math.pow(hsvIndex == 0 ? value : hsv.x, powers.x),
        math.pow(hsvIndex == 1 ? value : hsv.y, powers.y),
        math.pow(hsvIndex == 2 ? value : hsv.z, powers.z));
      return value == 1 ? defaultColor : color;
    }

    /// <summary>
    /// Get point inside bounds
    /// </summary>
    /// <param name="viewport"></param>
    /// <param name="bounds"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public static float2 GetViewportPosition(Viewport viewport, AABB bounds, float2 position) =>
      new float2(
        (position.x - bounds.Min.x) * viewport.Width / bounds.Size.x,
        (position.y - bounds.Min.y) * viewport.Height / bounds.Size.y) + viewport.Min.xy;

    /// <summary>
    /// Gets a screen position in viewport coordenates
    /// </summary>
    public static float2 GetScreenPointInsideViewport(AABB bounds, AABB viewport, float2 screenPosition) =>
      GetViewportPosition(
        viewport,
        bounds,
        screenPosition);
  }
}
