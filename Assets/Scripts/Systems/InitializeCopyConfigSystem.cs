using Mandelbrot;
using Mandelbrot.Jobs;
using Unity.Entities;
using Mandelbrot.Components;

[assembly: RegisterGenericJobType(typeof(ComponentCopyJob<ComponentCopy<Viewport>, Viewport>))]
[assembly: RegisterGenericComponentType(typeof(ComponentCopy<Viewport>))]
namespace Mandelbrot {
  public class InitializeCopyConfigSystem : InitializeCopyComponentSystem<Viewport> { }
}
