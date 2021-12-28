using Unity.Entities;
using UnityEngine;

namespace Mandelbrot {
  [GenerateAuthoringComponent]
  public struct TextureConfig : IComponentData {
    public int Width;
    public int Height;
    public TextureFormat TextureFormat;
    public FilterMode Filter;
  }
}