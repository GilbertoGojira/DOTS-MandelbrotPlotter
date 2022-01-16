using Unity.Entities;

namespace Mandelbrot.Components {
  public struct Stats : IComponentData {
    public long Iterations;
    public long Duration;
  }
}
