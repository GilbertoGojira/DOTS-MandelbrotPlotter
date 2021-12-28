using Unity.Entities;
using UnityEngine;

namespace Mandelbrot {
  [GenerateAuthoringComponent]
  public class TextureRef : IComponentData {
    public Texture2D Value;
  }
}
