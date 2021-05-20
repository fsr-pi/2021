using MVC.Models;
using System.Collections.Generic;

namespace MVC.ViewModels
{
    public class DokumentiViewModel
    {
        public IEnumerable<ViewDokumentInfo> Dokumenti { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public DokumentFilter Filter { get; set; }
    }
}
