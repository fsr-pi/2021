using System;

namespace Inheritance
{
  public class Motorbike : Vehicle
  {
    public bool HasSidecar { get; set; }
    public Motorbike(string model, double horsePower, bool sidecar)
     : base(model, horsePower)
    {
      Console.WriteLine($"Creating car {model}");
      this.HasSidecar = sidecar;
    }
    public override void Start()
    {
      Console.WriteLine("Start Motorbike " + Model);
    }
  }

}
