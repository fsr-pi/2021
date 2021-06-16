using CommandQueryCore;
using System.Collections.Generic;

namespace Contract.Queries
{
  public class SearchMjestoQuery : IQuery<IEnumerable<DTOs.Mjesto>>
  {
    public int? PostanskiBroj { get; set; }
    public string OznDrzave { get; set; }
    public string NazivMjesta { get; set; }
  }
}
