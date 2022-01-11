using Mandelbrot;
using Mandelbrot.Entities;
using Mandelbrot.UI;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

[assembly: RegisterGenericComponentType(typeof(UIGetAction<Iterations>))]
[assembly: RegisterGenericComponentType(typeof(UISetAction<Iterations>))]
[assembly: RegisterGenericComponentType(typeof(UIGetAction<Stats, TextureConfig, Viewport>))]

namespace Mandelbrot.UI {
  public class MandelbrotUI: MonoBehaviour, IConvertGameObjectToEntity {
    [SerializeField]
    GameObject _reference;
    [SerializeField]
    Slider _iterations;
    [SerializeField]
    TMP_Text _statsLabel;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
      var world = World.DefaultGameObjectInjectionWorld;
      var target = conversionSystem.GetPrimaryEntity(_reference);

      CreateUIActionsForIterations(world, entity, target);
      CreateUIActionsForStats(world, entity, target);
    }

    void CreateUIActionsForIterations(World world, Entity source, Entity target) {
      Utilities.CreateUIGetAction<Iterations>(world.EntityManager, source, GetIterations, target);
      _iterations.onValueChanged.AddListener(_ =>
        Utilities.CreateUISetAction<Iterations>(world.EntityManager, target, SetIterations));
      world.ForceGetOrCreateSystem<UISystem<Iterations>>();
    }

    void GetIterations(Iterations it) {
      _iterations.SetValueWithoutNotify(it.Value);
    }

    void SetIterations(ref Iterations it) {
      it.Value = (int)_iterations.value;
      Debug.Log($"Value changed to {it.Value}");
    }

    void CreateUIActionsForStats(World world, Entity source, Entity target) {
      Utilities.CreateUIGetAction<Stats, TextureConfig, Viewport>(world.EntityManager, source, GetStats, target);
      world.ForceGetOrCreateSystem<UISystem<Stats, TextureConfig, Viewport>>();
    }

    void GetStats(Entity entity, Stats stat, TextureConfig textureConfig, Viewport viewport) {
      var statsInfo =
            $"{entity}:\n" +
            $"Executed {stat.Iterations} iterations in {stat.Duration}ms.\n" +
            $"Average of {(float)stat.Iterations / (textureConfig.Width * textureConfig.Height)} iterations per point\n" +
            $"Resolution of {textureConfig.Width}x{textureConfig.Height} with {textureConfig.Width * textureConfig.Height} points and range {viewport}\n\n";
      Debug.Log(statsInfo);
      _statsLabel?.SetText(statsInfo);
    }
  }
}
