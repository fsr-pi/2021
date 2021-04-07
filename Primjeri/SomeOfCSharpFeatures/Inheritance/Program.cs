using System;

namespace Inheritance
{
  class Program
  {
    static void Main(string[] args)
    {
      Vehicle v = new Vehicle("BMW Isseta", 9.5);
      v.Start();
      
      Car car = new ElectricCar("Nissan Leaf", 148, 5, 40)
                    {
                      Range = 270
                    };
      Console.WriteLine("car model = " + car.Model);
      Console.WriteLine("car horse power = " + car.HorsePower);
      Console.WriteLine("car number of doors = " + car.Doors);
      if (car is ElectricCar)
      {
        var electric = (ElectricCar)car;
        Console.WriteLine("car battery = " + electric.BatteryCapacity);        
        if (electric.Range.HasValue)
        {
          Console.WriteLine("car range = " + electric.Range);
        }
      }

      car.BuckleSeatBelt();
      car.Start();
      car.Stop();

      ((Vehicle)car).Start();
      ((Vehicle)car).Stop();

      // objekt tipa Motocikl
      Motorbike moto = new Motorbike("Ural", 41, true);
      Console.WriteLine("motorbike model {0} ", moto.Model);
      Console.WriteLine("motorbike horse power = " + moto.HorsePower);
      Console.WriteLine($"motorbike has sidecar? = {moto.HasSidecar}");

      moto.Start();
      moto.Stop();

      ((Vehicle)moto).Start();
      ((Vehicle)moto).Stop();            
    }
  }
}
