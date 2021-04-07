using System;

namespace Inheritance
{
  /// <summary>
  /// Motor vehicle
  /// </summary>
  public class Vehicle
  {
    public string Model { get; private set; }
    public double HorsePower { get; set; }

    public Vehicle(string model, double horsePower)
    {
      Console.WriteLine("Creating vehicle " + model);
      this.Model = model;
      this.HorsePower = horsePower;
    }

    public void Stop()
    {
      Console.WriteLine("Stop vehicle " + Model);
    }

    public virtual void Start()
    {
      Console.WriteLine("Start vehicle " + Model);
    }
  }
}
