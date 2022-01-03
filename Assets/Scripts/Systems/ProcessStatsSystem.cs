using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Mandelbrot.UI {

  public class ProcessStatsSystem : SystemBase {
    MainUISystem _uiSystem;

    protected override void OnCreate() {
      base.OnCreate();
      _uiSystem = World.GetOrCreateSystem<MainUISystem>();
    }

    protected override void OnUpdate() {
      Entities
      .WithoutBurst()
      .WithChangeFilter<Stats>()
      .ForEach((Entity entity, in Stats stat, in Viewport viewport, in TextureConfig textureConfig) => {
        _uiSystem.AddPreAction((_, ui) => ui.StatsLabel?.SetText(string.Empty));
        var _stat = stat;
        var totalIterations = stat.Iterations;
        var _viewport = viewport;
        var _textureConfig = textureConfig;
        _uiSystem.AddAction((_, ui) => {
          var statsInfo =
#if UNITY_EDITOR
                $"{EntityManager.GetName(entity)} " +
#endif
            $"{entity}:\n" +
            $"Executed {totalIterations} iterations in {_stat.Duration}ms.\n" +
            $"Average of {(float)_stat.Iterations / (_textureConfig.Width * _textureConfig.Height)} iterations per point\n" +
            $"Resolution of {_textureConfig.Width}x{_textureConfig.Height} with {_textureConfig.Width * _textureConfig.Height} points and range {_viewport}\n\n";
          Debug.Log(statsInfo);
          if(ui.StatsLabel)
            ui.StatsLabel.text += statsInfo;
        });
      }).Run();
    }
  }
}
