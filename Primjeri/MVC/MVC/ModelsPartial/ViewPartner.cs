namespace MVC.Models
{
  public class ViewPartner
  {
    public int IdPartnera { get; set; }
    public string TipPartnera { get; set; }
    public string OIB { get; set; }
    public string Naziv { get; set; }
    public string TipPartneraText => TipPartnera == "O" ? "Osoba" : "Tvrtka";
  }
}
