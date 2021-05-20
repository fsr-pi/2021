using MVC.Models;
using System.Collections.Generic;

namespace MVC.ViewModels
{
  public class DrzaveViewModel
  {
    public IEnumerable<Drzava> Drzave { get; set; }
    public PagingInfo PagingInfo { get; set; }
  }
}
