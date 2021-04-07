using System;

namespace Extensions
{
  [Flags]
  public enum DayPeriod
  {
    Morning = 1, Evening = 2, Afternoon = 4, Night = 8
  }

  public class Activity
  {
    public string Person { get; set; }
    public int Hours { get; set; }
    public DayPeriod Period { get; set; }
  }
}
