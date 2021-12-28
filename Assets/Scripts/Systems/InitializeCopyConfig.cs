using Mandelbrot;
using Mandelbrot.Jobs;
using Unity.Entities;

[assembly: RegisterGenericJobType(typeof(ComponentCopyJob<ComponentCopy<Config>, Config>))]
namespace Mandelbrot {
  public class InitializeCopyConfig : InitializeCopyComponent<Config> { }
}
