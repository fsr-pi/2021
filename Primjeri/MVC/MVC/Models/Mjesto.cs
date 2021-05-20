using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVC.Models
{
  public partial class Mjesto
  {
    public Mjesto()
    {
      PartnerIdMjestaIsporukeNavigation = new HashSet<Partner>();
      PartnerIdMjestaPartneraNavigation = new HashSet<Partner>();
    }

    public int IdMjesta { get; set; }

    [Display(Name = "Država")]
    public string OznDrzave { get; set; }
    [Display(Name = "Naziv mjesta")]
    public string NazMjesta { get; set; }
    [Display(Name = "Poštanski broj mjesta")]
    public int PostBrMjesta { get; set; }
    [Display(Name = "Poštanski naziv mjesta")]
    public string PostNazMjesta { get; set; }

    public virtual Drzava OznDrzaveNavigation { get; set; }
    public virtual ICollection<Partner> PartnerIdMjestaIsporukeNavigation { get; set; }
    public virtual ICollection<Partner> PartnerIdMjestaPartneraNavigation { get; set; }
  }
}
