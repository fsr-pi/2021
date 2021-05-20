using System.Collections.Generic;

namespace MVC.ViewModels
{
  public class MjestaViewModel
  {
    public IEnumerable<MjestoViewModel> Mjesta { get; set; }
    public PagingInfo PagingInfo { get; set; }
  }
}
