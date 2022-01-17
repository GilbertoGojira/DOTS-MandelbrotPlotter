using Unity.Entities;
using UnityEngine;

namespace Mandelbrot.Components {
  [GenerateAuthoringComponent]
  public struct Config : IComponentData {
    [Tooltip("Calculate when start")]
    public bool CalculationReady;
    [Tooltip("The mandelbrot resolution")]
    public Resolution Resolution;
    [Tooltip("Number of max iteratiosn per point")]
    public Iterations Iterations;
    [Tooltip("Viewport where the mandelbrot set will be calculated")]
    public Viewport Viewport;
    [Tooltip("Color of the points for the mandelbrot set")]
    public HSVColor ColorSetup;
    [Tooltip("The zoom setup")]
    public Zoom Zoom;
  }
}
