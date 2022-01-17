using UnityEngine;
using Unity.Entities;

namespace Mandelbrot.Authoring {
  public class AnimationCurve : MonoBehaviour, IConvertGameObjectToEntity {
    [SerializeField]
    UnityEngine.AnimationCurve _curve;
    [SerializeField]
    int _samples;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
      var duration = _curve.length == 0 ? 0 : _curve[_curve.length - 1].time;
      var buffer = dstManager.AddBuffer<Components.AnimationCurve>(entity);
      for (var i = 0; i < _samples; ++i)
        buffer.Add(_curve.Evaluate(i * duration / _samples));
    }
  }
}
