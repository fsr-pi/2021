using CommandQueryCore;
using Contract.Queries;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.QueryHandlers
{
  public class SearchMjestoQueryHandler : IQueryHandler<SearchMjestoQuery, IEnumerable<Contract.DTOs.Mjesto>>
  {
    private readonly FirmaContext ctx;

    public SearchMjestoQueryHandler(FirmaContext ctx)
    {
      this.ctx = ctx;
    }
    public async Task<IEnumerable<Contract.DTOs.Mjesto>> Handle(SearchMjestoQuery query)
    {
      var dbQuery = ctx.Mjesto.AsQueryable();

      if (!string.IsNullOrWhiteSpace(query.NazivMjesta))
      {
        dbQuery = dbQuery.Where(m => m.NazMjesta == query.NazivMjesta);
      }
      if (!string.IsNullOrWhiteSpace(query.OznDrzave))
      {
        dbQuery = dbQuery.Where(m => m.OznDrzave == query.OznDrzave);
      }
      if (query.PostanskiBroj.HasValue)
      {
        dbQuery = dbQuery.Where(m => m.PostBrMjesta == query.PostanskiBroj.Value);
      }
      
      var list = await dbQuery.Select(m => new Contract.DTOs.Mjesto
                              {
                                IdMjesta = m.IdMjesta,
                                NazivDrzave = m.OznDrzaveNavigation.NazDrzave,
                                NazivMjesta = m.NazMjesta,
                                OznDrzave = m.OznDrzave,
                                PostBrojMjesta = m.PostBrMjesta,
                                PostNazivMjesta = m.PostNazMjesta
                              })
                              .ToListAsync();
      return list;
    }
  }
}
