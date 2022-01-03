using Mandelbrot.Math;

namespace Mandelbrot {

  public static class MandelbrotSet {

    /// <summary>
    /// The Mandelbrot set is a compact set, since it is closed and contained in the closed disk of radius 2 around the origin.
    /// </summary>
    private const int SQR_RADIUS = 4;

    /// <summary>
    /// Calculates the number of iterations required to exit
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="max">Max number of iterations</param>
    /// <returns>The the number of iterations required to exit</returns>
    public static int Calculate(double x, double y, int max = 255) {
      // Mandelbrot Zn+1 = Zn^2 + c
      var c = new Complex(x, y);
      var z = new Complex();
      for (var n = 1; n <= max; ++n) {
        z = z * z + c;
        // M set belongs in the circle of radius 2
        if (z.SqrLength > SQR_RADIUS)
          return n;
      }
      return max;
    }
  }
}
