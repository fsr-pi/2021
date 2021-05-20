using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVC.Models
{
  public partial class Drzava
  {
    public Drzava()
    {
      Mjesto = new HashSet<Mjesto>();
    }

    [Display(Name = "Oznaka države")]
    public string OznDrzave { get; set; }

    [Display(Name = "Naziv države", Prompt = "Unesite naziv")]
    public string NazDrzave { get; set; }

    [Display(Name = "ISO3 oznaka")]
    public string Iso3drzave { get; set; }

    [Display(Name = "Šifra države")]
    public int? SifDrzave { get; set; }

    public virtual ICollection<Mjesto> Mjesto { get; set; }
  }
}
