using System;
using System.Collections.Generic;
using System.Text;

namespace PropertiesIndexersRefOut
{
  public class Triple
  {
    private int first;
    public int First
    {
      get
      {
        return first;
      }
      set
      {
        first = value;
      }
    }

    public int Second { get; set; }

    public int Third => First + Second;
    public override string ToString()
    {
      return $"({First}, {Second}, {Third})";
    }

    #region Indexer
    public int this[string s, int position]  
    {      
      get
      {        
        int num;
        switch (s)
        {
          case "A":
            num = this.First;
            break;
          case "B":
            num = this.Second;
            break;
          case "C":
            num = this.Third;
            break;
          default:
            throw new ArgumentException("Invalid position in triple (should be A, B, or C)");
        }
        int digit = 0;
        while(num > 0 && position > 0)
        {
          digit = num % 10;
          num /= 10;
          position--;
        }
        return digit;
      }
    }
    #endregion


    #region Operator overloading
    public static Triple operator +(Triple x, Triple y)
    {
      // prođe kroz gettere argumenata i konstruktor rezultata
      return new Triple
      {
        First = x.First + y.First,
        Second = x.Second + y.Second
      };
    }
    #endregion




  }
}
