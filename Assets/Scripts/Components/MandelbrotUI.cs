using TMPro;
using Unity.Entities;
using UnityEngine.UI;

namespace Mandelbrot.UI {
  [GenerateAuthoringComponent]
  public class MandelbrotUI: IComponentData {
    public Slider Iterations;
    public TMP_Text StatsLabel;
  }
}
