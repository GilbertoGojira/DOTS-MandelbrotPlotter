using Mandelbrot.Collections;
using Mandelbrot.Components;
using Mandelbrot.Math;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Mandelbrot.Jobs {
  [BurstCompile]
  struct GenerateColorJob : IJobFor {
    public Config Config;
    public double2 Step;
    public int Width;
    public int Height;
    [WriteOnly]
    [NativeDisableParallelForRestriction]
    [NativeDisableContainerSafetyRestriction]
    public NativeArray<Color32> Colors;
    public NativeCounter.Concurrent TotalIterations;

    public void Execute(int x) {
      var lastColor = default(Color32);
      var lastN = -1;
      var rx = Config.Viewport.Min.x + (x * Step.x);
      for (var y = 0; y < Height; ++y) {
        // So we need to convert the coordinate to be inside that range
        // Before we supply it to Mandelbrot
        var ry = Config.Viewport.Min.y + (y * Step.y);
        var n = MandelbrotSet.Calculate(rx, ry, Config.Iterations.Value);
        lastColor = lastN != n ? Utilities.GetColor((float)n / Config.Iterations.Value, 0, Config.ColorSetup.HSV, Config.ColorSetup.HSVPower, Config.ColorSetup.MainColor) : lastColor;
        Colors[y * Width + x] = lastColor;
        lastN = n;
        TotalIterations.Increment(n);
      }
    }
  }
}