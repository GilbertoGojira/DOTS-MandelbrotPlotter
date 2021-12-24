namespace Mandelbrot.Math {

  public struct Complex {
    public double Real { get; private set; }
    public double Imaginary { get; private set; }

    public Complex(double real, double imaginary) {
      Real = real;
      Imaginary = imaginary;
    }

    public double SqrLength {
      get => Real * Real + Imaginary * Imaginary;
    }

    public static Complex operator *(Complex a, Complex b) =>
      new Complex(
        a.Real * b.Real - a.Imaginary * b.Imaginary,
        a.Real * b.Imaginary + a.Imaginary * b.Real);

    public static Complex operator +(Complex a, Complex b) =>
      new Complex(a.Real + b.Real, a.Imaginary + b.Imaginary);
  }
}
