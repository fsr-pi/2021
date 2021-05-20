using System;
using System.Collections.Generic;

namespace MVC.Models
{
    public partial class Osoba
    {
        public int IdOsobe { get; set; }
        public string PrezimeOsobe { get; set; }
        public string ImeOsobe { get; set; }

        public virtual Partner IdOsobeNavigation { get; set; }
    }
}
