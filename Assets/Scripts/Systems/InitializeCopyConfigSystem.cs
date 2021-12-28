using Mandelbrot;
using Mandelbrot.Jobs;
using Unity.Entities;

[assembly: RegisterGenericJobType(typeof(ComponentCopyJob<ComponentCopy<Config>, Config>))]
[assembly: RegisterGenericComponentType(typeof(ComponentCopy<Config>))]
namespace Mandelbrot {
  public class InitializeCopyConfigSystem : InitializeCopyComponentSystem<Config> { }
}
