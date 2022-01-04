using System;
using Unity.Mathematics;
using Unity.Entities;

namespace Mandelbrot {
  [Serializable]
  public struct Viewport : IComponentData {
    public AABB Value;

    public float3 Min {
      get => Value.Min;
    }

    public float3 Max {
      get => Value.Max;
    }

    public float Width {
      get => Value.Size.x;
      set {
        var extents = Value.Extents;
        extents.x = value * 0.5f;
        Value.Extents = extents;
      }
    }

    public float Height {
      get => Value.Size.y;
      set {
        var extents = Value.Extents;
        extents.y = value * 0.5f;
        Value.Extents = extents;
      }
    }

    public override string ToString() =>
      Value.ToString();

    public static implicit operator Viewport(float3 v) =>
      new Viewport {
        Value = new MinMaxAABB { Min = v, Max = v }
      };

    public static implicit operator Viewport(float2 v) =>
      new Viewport {
        Value = new MinMaxAABB { Min = new float3(v.xy, 0), Max = new float3(v.xy, 0) }
      };

    public static implicit operator Viewport(AABB v) =>
      new Viewport { Value = v };

    public static implicit operator Viewport(MinMaxAABB v) =>
      new Viewport { Value = v };
  }
}
