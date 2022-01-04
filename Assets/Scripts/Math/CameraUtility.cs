using Unity.Mathematics;
using UnityEngine;

namespace Mandelbrot.Math {
  public class CameraUtility {

    private static float m_screenDpi = -1f;
    private static float m_fallbackDpi = 96f;

    public static float ScreenDpi {
      get {
        m_screenDpi = m_screenDpi < 0f ? Screen.dpi : m_screenDpi;
        return math.max(m_screenDpi, m_fallbackDpi);
      }
    }

    /// <summary>
    /// Thread safe calc Screen rect
    /// </summary>
    /// <param name="projectionMatrix"></param>
    /// <param name="worldToCameraMatrix"></param>
    /// <param name="pos"></param>
    /// <param name="screenSize"></param>
    /// <returns></returns>
    public static float2 WorldToScreenPoint(float4x4 projectionMatrix, float4x4 worldToCameraMatrix, float3 pos, float2 screenSize) {
      var world2Screen = math.mul(projectionMatrix, worldToCameraMatrix);
      var screenPos = world2Screen.MultiplyPoint(pos);
      // (-1, 1)'s clip => (0 ,1)'s viewport
      screenPos = new float3(screenPos.x + 1f, screenPos.y + 1f, screenPos.z + 1f) / 2f;
      // viewport => screen
      return new float2(screenPos.x * screenSize.x, screenPos.y * screenSize.y);
    }

    public static float3 ScreenToWorldPoint(float4x4 projectionMatrix, float4x4 worldToCameraMatrix, float4x4 localToWorldMatrix, float3 screenPos, float2 screenSize) {
      var world2Screen = math.mul(math.mul(projectionMatrix, worldToCameraMatrix), localToWorldMatrix);
      var screen2World = math.inverse(math.mul(projectionMatrix, worldToCameraMatrix));
      var depth = world2Screen.MultiplyPoint(screenPos).z;
      // viewport pos (0 ,1)
      var viewPos = new float3(screenPos.x / screenSize.x, screenPos.y / screenSize.y, (depth + 1f) / 2f);
      // clip pos (-1, 1) 
      var clipPos = viewPos * 2f - new float3(1);
      // world pos
      return screen2World.MultiplyPoint(clipPos);
    }

    public static Ray ScreenPointToRay(float4x4 projectionMatrix, float4x4 worldToCameraMatrix, float4x4 localToWorldMatrix, float3 forward, float3 screenPos, float2 screenSize) {
      return new Ray(ScreenToWorldPoint(projectionMatrix, worldToCameraMatrix, localToWorldMatrix, screenPos, screenSize), forward);
    }

    public static AABB GetAABB(Bounds bounds) {
      return new AABB { Center = bounds.center, Extents = bounds.extents };
    }
  }
}