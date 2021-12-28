using Unity.Entities;
using Unity.Jobs;
using Unity.Rendering;

namespace Mandelbrot {
  public class CopyToRenderMesh : SystemBase {
    protected override void OnUpdate() {
      Entities
        .WithoutBurst()
        .WithChangeFilter<TextureRef>()
        .ForEach((RenderMesh mesh, TextureRef texture) =>
          mesh.material.mainTexture = texture.Value)
        .Run();
    }
  }
}
