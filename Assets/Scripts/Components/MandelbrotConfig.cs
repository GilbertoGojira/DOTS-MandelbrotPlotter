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
    [Tooltip("The factor that will be applied to the mandelbrot viewport (eg. 2 will double it's dimensions, 0.5 will reduce by half)")]
    public float ZoomInFactor;
    [Tooltip("The factor that will be applied to the mandelbrot viewport (eg. 2 will double it's dimensions, 0.5 will reduce by half)")]
    public float ZoomOutFactor;
    [Tooltip("The duration in seconds for the zoom")]
    public float ZoomDuration;
  }
}
