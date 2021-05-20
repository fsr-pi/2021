using System.ComponentModel.DataAnnotations;

namespace MVC.ViewModels
{
  public class StavkaViewModel
  {
    public int IdStavke { get; set; }
    public int SifArtikla { get; set; }
    public string NazArtikla { get; set; }
    [Range(1e-6, double.MaxValue, ErrorMessage = "Količina mora biti pozitivan broj")]
    public decimal KolArtikla { get; set; }
    public decimal JedCijArtikla { get; set; }
    [Range(0, 1, ErrorMessage = "Rabat mora biti između 0 i 1")]
    public decimal PostoRabat { get; set; }

    public decimal IznosArtikla
    {
      get
      {
        return KolArtikla * JedCijArtikla * (1 - PostoRabat);
      }
    }
  }
}
