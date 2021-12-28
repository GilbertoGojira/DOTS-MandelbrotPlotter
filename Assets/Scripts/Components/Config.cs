using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Mandelbrot {
  [GenerateAuthoringComponent]
  public struct Config : IComponentData {
    [Tooltip("Number of max iteratiosn per point")]
    public int Iterations;
    [Tooltip("Viewport where the mandelbrot set will be calculated")]
    public Viewport Viewport;
    [Tooltip("Color of the points inside the mandelbrot set")]
    public Color MandelbrotColor;
    [Range(0, 1)]
    public float H;
    [Range(0, 1)]
    public float S;
    [Range(0, 1)]
    public float V;

    public float HPower;
    public float SPower;
    public float VPower;

    public float3 HSV {
      get => new float3(H, S, V);
    }

    public float3 HSVPower {
      get => new float3(HPower, SPower, VPower);
    }
  }
}
