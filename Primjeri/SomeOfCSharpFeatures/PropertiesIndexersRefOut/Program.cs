using System;

namespace PropertiesIndexersRefOut
{
  class Program
  {
    static void Main(string[] args)
    {
      Triple t1 = new Triple
      {
        First = 456,
        Second = 789
      };

      Triple t2 = new Triple
      {
        First = 1000,
        Second = 2000
      };

      Console.WriteLine("t1 + t2 = " + (t1 + t2));

      CreateRandomTriple(t1, ref t2, out Triple t3);
      
      Console.WriteLine("t1 = " + t1);
      Console.WriteLine("t2 = " + t2);
      Console.WriteLine("t3 = " + t3);
      
      Console.WriteLine(t1["A", 1]);
      Console.WriteLine(t1["B", 2]);
      Console.WriteLine(t1["B", 3]);

      PrintTriples(t1, t2, t3);
    }

    private static void PrintTriples(params Triple[] triples)
    {
      foreach(var t in triples)
      {
        Console.WriteLine(t);
      }
    }

    private static void CreateRandomTriple(Triple t1, ref Triple t2, out Triple t3)
    {
      Random r = new Random();
      t3 = new Triple
      {
        First = r.Next(t1.First, t2.Second),
        Second = r.Next(maxValue: t2.Second, minValue: t1.First)
      };

      t1 = t3;
      t2 = t3;
    }
  }
}