using System;
using System.Collections.Generic;
using System.Text;

namespace Inheritance
{
  public class ElectricCar : Car
  {
    public int BatteryCapacity { get; set; }
    public int? Range { get; set; }

    public ElectricCar(string model, double horsePower, int doors, int batteryCapacity) :
      base(model, horsePower, doors)
    {
      this.BatteryCapacity = batteryCapacity;
    }
  }
}
