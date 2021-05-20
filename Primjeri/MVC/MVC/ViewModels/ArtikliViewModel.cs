using System.Collections.Generic;

namespace MVC.ViewModels
{
  public class ArtikliViewModel
  {
    public IEnumerable<ArtiklViewModel> Artikli { get; set; }
    public PagingInfo PagingInfo { get; set; }
  }
}
