using Mandelbrot.Components;
using Unity.Entities;
using Unity.Jobs;

namespace Mandelbrot {
  public class CopyToTextureSystem : SystemBase {

    protected override void OnUpdate() {
      // Add TextureRef to all Entities with TextureConfig
      Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .WithNone<TextureRef>()
        .ForEach((Entity entity, in Resolution config) =>
          EntityManager.AddComponentObject(entity, new TextureRef())
        ).Run();

      // When TextureConfig changed will recreate the underlying texture object
      // with proper config
      Entities
        .WithoutBurst()
        .WithChangeFilter<Resolution, TextureConfig>()
        .ForEach((Entity entity, TextureRef texture, in Resolution resolution, in TextureConfig config) => {
          UnityEngine.Object.Destroy(texture.Value);
          texture.Value = new UnityEngine.Texture2D(resolution.Width, resolution.Height, config.TextureFormat, false);
          // TODO: Maybe we should have this attribute in another component to avoid creating
          // a new texture everytime we change it
          texture.Value.filterMode = config.Filter;
        }).Run();

      // When PointColor changes will copy those into the underlying texture object
      Entities
        .WithoutBurst()
        .WithChangeFilter<PointColor>()
        .ForEach((Entity entity, TextureRef texture, DynamicBuffer<PointColor> colors) => {
          if (colors.IsEmpty) return;
          texture.Value.LoadRawTextureData(colors.AsNativeArray());
          texture.Value.Apply();
      }).Run();
    }
  }
}
