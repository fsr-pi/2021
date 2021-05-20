using MVC.Models;
using System.Collections.Generic;

namespace MVC.ViewModels
{
  public class PartneriViewModel
  {
    public IEnumerable<ViewPartner> Partneri { get; set; }
    public PagingInfo PagingInfo { get; set; }
  }
}
