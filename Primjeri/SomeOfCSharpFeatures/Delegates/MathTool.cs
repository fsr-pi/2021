using System;

namespace Delegates
{
  public class MathTool
  {
    public static int Sum(int x, int y)
    {
      return x + y;
    }

    public static int Diff(int x, int y)
    {
      return x - y;
    }

    public static void PrintSquare(int x)
    {
      Console.WriteLine("x^2 = " + x * x);
    }

    public static void PrintSquareRoot(int x)
    {
      Console.WriteLine("sqrt(x) = " + Math.Sqrt(x));
    }
  }
}
