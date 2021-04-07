using System;

namespace Delegates
{
  class Program
  {
    public delegate int MathFunction(int a, int b);
    public delegate void PrintFunction(int n);
    static void Main(string[] args)
    {
      int x = 16, y = 2;
      MathFunction mf = MathTool.Sum;
      Console.WriteLine("mf({0}, {1}) = {2}", x, y, mf(x, y));
      mf = MathTool.Diff;
      Console.WriteLine("mf({0}, {1}) = {2}", x, y, mf(x, y));
      PrintFunction pf = MathTool.PrintSquare;
      pf += MathTool.PrintSquareRoot;
      pf(x);
      pf -= MathTool.PrintSquare;
      Console.WriteLine();
      pf(y);

      Func<int, int, int> func = MathTool.Sum;
      Console.WriteLine("func({0}, {1}) = {2}", x, y, func(x, y));
      
      Action<int> action = MathTool.PrintSquare;
      action(x);
    }
  }
}
