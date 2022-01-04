using Unity.Entities;
using Unity.Mathematics;

namespace Mandelbrot {
  public enum InputType {
    NONE,
    LEFT_CLICK,
    RIGHT_CLICK
  }

  public struct InputStatus : IComponentData {
    public InputType Type;
    public float2 Position;
  }
}
