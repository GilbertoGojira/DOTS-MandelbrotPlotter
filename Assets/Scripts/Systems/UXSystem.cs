using Unity.Entities;
using UnityEngine;
namespace Mandelbrot.UI {
  public class UXSystem : SystemBase {
    protected override void OnUpdate() {
      Entities
        .WithoutBurst()
        .WithChangeFilter<Stats>()
        .ForEach((Entity entity, in Stats stat, in Config config, in TextureConfig textureConfig) => {
          var totalIterations = stat.Iterations;
          var statsInfo =
#if UNITY_EDITOR
            $"{EntityManager.GetName(entity)} " +
#endif
            $"{entity}:\n" +
            $"Executed {totalIterations} iterations in {stat.Duration}ms.\n" +
            $"Average of {(float)totalIterations / (textureConfig.Width * textureConfig.Height)} iterations per point\n" +
            $"Resolution of {textureConfig.Width}x{textureConfig.Height} with {textureConfig.Width * textureConfig.Height} points and range {config.Viewport}";
          Debug.Log(statsInfo);
        }).Run();
    }
  }
}