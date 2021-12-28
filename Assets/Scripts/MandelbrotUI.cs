using TMPro;
using Unity.Entities;

namespace Mandelbrot.UI {
  [GenerateAuthoringComponent]
  public class MandelbrotUI: IComponentData {
    public TMP_Text StatsLabel;
  }
}
