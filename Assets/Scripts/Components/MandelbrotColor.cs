using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Mandelbrot {
  [Serializable]
  public struct MandelbrotColor : IComponentData {
    public Color MainColor;
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