using Unity.Entities;
using Mandelbrot.Entities;

namespace Mandelbrot.UI {

  public abstract class UISystemBase<TCOMP> : SystemBase
    where TCOMP : IComponentData {
    protected UISystem<TCOMP> UISystem;

    protected override void OnCreate() {
      base.OnCreate();
      UISystem = World.ForceGetOrCreateSystem<UISystem<TCOMP>>();
    }
  }
}
