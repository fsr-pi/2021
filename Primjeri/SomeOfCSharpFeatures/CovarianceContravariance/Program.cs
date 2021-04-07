using Inheritance;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CovarianceContravariance
{
  class Program
  {
    static void Main(string[] args)
    {
      List<Car> cars = new List<Car>{
        new Car("Coupe", 200, 3),
        new Car("Wagon", 100, 5),
        new Car("Limousine", 150, 4),
        new ElectricCar("Electric Hatchback", 148, 5, 40)
      };
      PrintVehicles(cars);
      
      Comparison<Vehicle> comparisonFunction = (a, b) => -a.HorsePower.CompareTo(b.HorsePower);
      IComparer<Vehicle> comparer = Comparer<Vehicle>.Create(comparisonFunction);

      PrintBetterCar(cars[2], cars[3], comparer);
      PrintBetterCar(cars.Skip(2).First(), cars.Last(), comparisonFunction);

      PrintBetterCar(cars[2], cars[3], (a, b) => b.Doors - a.Doors);
    }

    static void PrintVehicles(IEnumerable<Vehicle> vehicles)
    {
      foreach (var vehicle in vehicles)
      {
        Console.WriteLine("\t " + vehicle.Model);
      }
    }

    static void PrintBetterCar(Car a, Car b, IComparer<Car> comparer)
    {
      int result = comparer.Compare(a, b);
      string betterModel = result <= 0 ? a.Model : b.Model;
      string worseModel = result <= 0 ? b.Model : a.Model;
      Console.WriteLine($"\t{betterModel} is better or equal than {worseModel}");
    }

    static void PrintBetterCar(Car a, Car b, Comparison<Car> comparer)
    {
      int result = comparer(a, b);
      string betterModel = result <= 0 ? a.Model : b.Model;
      string worseModel = result <= 0 ? b.Model : a.Model;
      Console.WriteLine($"\t{betterModel} is better or equal than {worseModel}");
    }
  }
}
