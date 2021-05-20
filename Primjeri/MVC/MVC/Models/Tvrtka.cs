using System;
using System.Collections.Generic;

namespace MVC.Models
{
    public partial class Tvrtka
    {
        public int IdTvrtke { get; set; }
        public string MatBrTvrtke { get; set; }
        public string NazivTvrtke { get; set; }

        public virtual Partner IdTvrtkeNavigation { get; set; }
    }
}
