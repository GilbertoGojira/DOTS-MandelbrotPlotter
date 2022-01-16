using Unity.Entities;
using UnityEngine;

namespace Mandelbrot {
  [GenerateAuthoringComponent]
  public struct TextureConfig : IComponentData {
    public TextureFormat TextureFormat;
    public FilterMode Filter;
  }
}
