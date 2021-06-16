using Contract.Queries;
using Contract.QueryHandlers;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DAL.QueryHandlers
{
  public class MjestaQueryHandler : IMjestaQueryHandler
  {
    private static Dictionary<string, Expression<Func<DAL.Models.Mjesto, object>>> orderSelectors = new Dictionary<string, Expression<Func<Mjesto, object>>>
    {
      [nameof(Contract.DTOs.Mjesto.IdMjesta).ToLower()] = m => m.IdMjesta,
      [nameof(Contract.DTOs.Mjesto.NazivDrzave).ToLower()] = m => m.OznDrzaveNavigation.NazDrzave,
      [nameof(Contract.DTOs.Mjesto.NazivMjesta).ToLower()] = m => m.NazMjesta,
      [nameof(Contract.DTOs.Mjesto.OznDrzave).ToLower()] = m => m.OznDrzave,
      [nameof(Contract.DTOs.Mjesto.PostBrojMjesta).ToLower()] = m => m.PostBrMjesta,
      [nameof(Contract.DTOs.Mjesto.PostNazivMjesta).ToLower()] = m => m.PostNazMjesta
    };

    private readonly FirmaContext ctx;


    public MjestaQueryHandler(FirmaContext ctx)
    {
      this.ctx = ctx;
    }

    public async Task<IEnumerable<Contract.DTOs.Mjesto>> Handle(MjestaQuery query)
    {
      var dbQuery = ctx.Mjesto.AsQueryable();

      if (!string.IsNullOrWhiteSpace(query.SearchText))
      {
        dbQuery = dbQuery.Where(m => m.NazMjesta.Contains(query.SearchText));
      }

      if (query.Sort?.ColumnOrder != null)
      {
        foreach(var sortInfo in query.Sort.ColumnOrder)
        {
          if (orderSelectors.TryGetValue(sortInfo.Key.ToLower(), out var expr))
          {
            dbQuery = sortInfo.Value == SortInfo.Order.ASCENDING ? dbQuery.OrderBy(expr) : dbQuery.OrderByDescending(expr);
          }
        }
       
      }

      if (query.From.HasValue)
      {
        dbQuery = dbQuery.Skip(query.From.Value);
      }

      if (query.Count.HasValue)
      {
        dbQuery = dbQuery.Take(query.Count.Value);
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
