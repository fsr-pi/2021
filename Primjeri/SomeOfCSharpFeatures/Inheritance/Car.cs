using System;
using System.Collections.Generic;
using System.Text;

namespace Inheritance
{
  public class Car : Vehicle
  {
    public int Doors { get; set; }
    public string Color { get; set; } = "White";

    public Car(string model, double horsePower, int doors) 
      : base(model, horsePower)
    {
      Console.WriteLine($"Creating car {model}");
      this.Doors = doors;
    }

    public void BuckleSeatBelt()
    {
      Console.WriteLine("Car {0} - seat belt buckled", Model);
    }

    public override void Start()
    {
      Console.WriteLine("Start Car " + Model);
    }

    public new void Stop()
    {
      Console.WriteLine("Stop Car " + Model);
    }
  }
}
