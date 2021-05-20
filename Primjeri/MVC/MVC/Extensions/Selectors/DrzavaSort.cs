using MVC.Models;
using System;
using System.Linq;

namespace MVC.Extensions.Selectors
{
  public static class DrzavaSort
  {
    public static IQueryable<Drzava> ApplySort(this IQueryable<Drzava> query, int sort, bool ascending)
    {
      System.Linq.Expressions.Expression<Func<Drzava, object>> orderSelector = null;
      switch (sort)
      {
        case 1:
          orderSelector = d => d.OznDrzave;
          break;
        case 2:
          orderSelector = d => d.NazDrzave;
          break;
        case 3:
          orderSelector = d => d.Iso3drzave;
          break;
        case 4:
          orderSelector = d => d.SifDrzave;
          break;
      }
      if (orderSelector != null)
      {
        query = ascending ?
               query.OrderBy(orderSelector) :
               query.OrderByDescending(orderSelector);
      }

      return query;
    }
  }
}
