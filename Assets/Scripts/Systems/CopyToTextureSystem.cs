using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Mandelbrot {
  public class CopyToTextureSystem : SystemBase {

    protected override void OnUpdate() {
      // Add TextureRef to all Entities with TextureConfig
      Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .WithNone<TextureRef>()
        .ForEach((Entity entity, in TextureConfig config) =>
          EntityManager.AddComponentObject(entity, new TextureRef())
        ).Run();

      // When TextureConfig changed will recreate the underlying texture object
      // with proper config
      Entities
        .WithoutBurst()
        .WithChangeFilter<TextureConfig>()
        .ForEach((Entity entity, TextureRef texture, in TextureConfig config) => {
          Object.Destroy(texture.Value);
          texture.Value = new Texture2D(config.Width, config.Height, config.TextureFormat, false);
          // TODO: Maybe we should have this attribute in another component to avoid creating
          // a new texture everytime we change it
          texture.Value.filterMode = config.Filter;
        }).Run();

      // When PointColor changes will copy those into the underlying texture object
      Entities
        .WithoutBurst()
        .WithChangeFilter<PointColor>()
        .ForEach((Entity entity, TextureRef texture, DynamicBuffer<PointColor> colors) => {
          texture.Value.LoadRawTextureData(colors.AsNativeArray());
          texture.Value.Apply();
      }).Run();
    }
  }
}
