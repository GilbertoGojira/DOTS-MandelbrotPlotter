using Unity.Entities;

namespace Mandelbrot {
  public struct Stats : IComponentData {
    public long Iterations;
    public long Duration;
  }
}
