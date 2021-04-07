using System;
using System.Collections.Generic;
using System.Linq;

namespace Extensions
{
  public enum Days
  {
    Monday, Tuesday, Wednesday,
    Thursday, Friday, Saturday, Sunday
  }
 
  class Program
  {    
    static void Main(string[] args)
    {
      var dict = new Dictionary<Days, List<Activity>>();
      Activity a = new Activity
      {
        Person = "John",
        Hours = 3,
        Period = DayPeriod.Morning
      };
      dict.GetOrCreate(Days.Monday).Add(a);

      a = new Activity
      {
        Person = "Mary",
        Hours = 12,
        Period = DayPeriod.Evening | DayPeriod.Night
      };
      dict.GetOrCreate(Days.Monday).Add(a);

      a = new Activity
      {
        Person = "Peter",
        Hours = 8,
        Period = DayPeriod.Night
      };
      dict.GetOrCreate(Days.Tuesday).Add(a);

      a = new Activity
      {
        Person = "Lisa",
        Hours = 5,
        Period = DayPeriod.Night | DayPeriod.Morning
      };
      dict.GetOrCreate(Days.Monday).Add(a);

      int sum = SumNightActivity(dict);
      Console.WriteLine(sum);

    }

    private static int SumNightActivity(Dictionary<Days, List<Activity>> dict)
    {
      int sum = 0;
      foreach(var list in dict.Values)
      {
        sum += list.Where(a => (a.Period & DayPeriod.Night) == DayPeriod.Night)
                   .Sum(a => a.Hours);                   
      }
      return sum;  
    }
  }
}
