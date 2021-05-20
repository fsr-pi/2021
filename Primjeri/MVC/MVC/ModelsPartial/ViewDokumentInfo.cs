using MVC.Util;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models
{
  public class ViewDokumentInfo
  {
    public int IdDokumenta { get; set; }

    [ExcelFormat("0.00%")]
    public decimal PostoPorez { get; set; }
    public int? IdPrethDokumenta { get; set; }

    [Display(Name = "Datum dokumenta")]
    [ExcelFormat("dd.mm.yyyy")]
    public DateTime DatDokumenta { get; set; }
    public int IdPartnera { get; set; }
    public string NazPartnera { get; set; }

    [ExcelFormat("#,###,##0.00")]
    public decimal IznosDokumenta { get; set; }
    public string VrDokumenta { get; set; }
    public int BrDokumenta { get; set; }

    [NotMapped]    
    public int Position { get; set; } //Position in the result
  }
}
