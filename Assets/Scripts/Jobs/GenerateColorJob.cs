using Mandelbrot.Collections;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Mandelbrot.Jobs {
  //[BurstCompile]
  struct GenerateColorJob : IJobFor {
    public double2 Step;
    public int Width;
    public int Height;
    public Viewport Viewport;
    public int Iterations;
    public float3 HSVBase;
    public float3 HSVPower;
    public Color32 DefaultColor;
    [WriteOnly]
    [NativeDisableParallelForRestriction]
    public NativeArray<Color32> Colors;
    public NativeCounter.Concurrent TotalIterations;

    public void Execute(int x) {
      var lastColor = default(Color32);
      var lastN = -1;
      var rx = Viewport.Min.x + (x * Step.x);
      for (var y = 0; y < Height; ++y) {
        // So we need to convert the coordinate to be inside that range
        // Before we supply it to Mandelbrot
        var ry = Viewport.Min.y + (y * Step.y);
        var n = MandelbrotSet.Calculate(rx, ry, Iterations);
        lastColor = lastN != n ? Utilities.GetColor((float)n / Iterations, 0, HSVBase, HSVPower, DefaultColor) : lastColor;
        Colors[y * Width + x] = lastColor;
        lastN = n;
        TotalIterations.Increment(n);
      }
    }
  }
}