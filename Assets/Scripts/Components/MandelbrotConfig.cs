using Unity.Entities;
using UnityEngine;

namespace Mandelbrot {
  [GenerateAuthoringComponent]
  public struct MandelbrotConfig : IComponentData {
    [Tooltip("Number of max iteratiosn per point")]
    public Iterations Iterations;
    [Tooltip("Viewport where the mandelbrot set will be calculated")]
    public Viewport Viewport;
    [Tooltip("Color of the points for the mandelbrot set")]
    public MandelbrotColor ColorSetup;
  }
}
