using System;
using Unity.Entities;
using UnityEngine;

namespace Mandelbrot.Components {
  [Serializable]
  public struct Zoom : IComponentData {
    [Tooltip("The factor that will be applied to the mandelbrot viewport (eg. 2 will double it's dimensions, 0.5 will reduce by half)")]
    public float InFactor;
    [Tooltip("The factor that will be applied to the mandelbrot viewport (eg. 2 will double it's dimensions, 0.5 will reduce by half)")]
    public float OutFactor;
    [Tooltip("The duration in seconds for the zoom")]
    public float Duration;
  }
}