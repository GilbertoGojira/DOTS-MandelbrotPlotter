using System;
using Unity.Mathematics;
using Unity.Entities;

namespace Mandelbrot {
  [Serializable]
  public struct Viewport : IComponentData {
    public double2 Min;
    public double2 Max;

    public double2 Center {
      get => Min + (Max - Min) / 2;
    }

    public double Width {
      get => Max.x - Min.x;
      set {
        Min.x = Center.x - value / 2;
        Max.x = Center.x + value / 2;
      }
    }

    public double Height {
      get => Max.y - Min.y;
      set {
        Min.y = Center.y - value / 2;
        Max.y = Center.y + value / 2;
      }
    }

    public static implicit operator Viewport(double2 v) => new Viewport { Min = v, Max = v };
  }
}
