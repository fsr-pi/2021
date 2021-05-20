using Microsoft.AspNetCore.Mvc;
using MVC.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVC.Models
{
  public partial class Artikl
  {
    public Artikl()
    {
      Stavka = new HashSet<Stavka>();
    }

    [Required(ErrorMessage = "Potrebno je uniijeti šifru artikla")]
    [Remote(action: nameof(ArtiklController.ProvjeriSifruArtikla), controller: "Artikl", ErrorMessage = "Artikl s navedenom šifrom već postoji")]
    public int SifArtikla { get; set; }

    [Required(ErrorMessage = "Potrebno je uniijeti naziv artikla")]
    public string NazArtikla { get; set; }
    public string JedMjere { get; set; }

    [Required(ErrorMessage = "Potrebno je uniijeti cijenu artikla")]
    [Range(0, double.MaxValue, ErrorMessage = "Cijena artikla mora biti nenegativan broj")]
    public decimal CijArtikla { get; set; }
    public bool ZastUsluga { get; set; }
    public string TekstArtikla { get; set; }
    public byte[] SlikaArtikla { get; set; }
    public int? SlikaChecksum { get; set; }

    public virtual ICollection<Stavka> Stavka { get; set; }
  }
}
