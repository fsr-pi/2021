using MVC.Models;
using System;
using System.Linq;

namespace MVC.Extensions.Selectors
{
  public static class MjestoSort
  {
    public static IQueryable<Mjesto> ApplySort(this IQueryable<Mjesto> query, int sort, bool ascending)
    {
      System.Linq.Expressions.Expression<Func<Mjesto, object>> orderSelector = null;
      switch (sort)
      {
        case 1:
          orderSelector = m => m.PostBrMjesta;
          break;
        case 2:
          orderSelector = m => m.NazMjesta;
          break;
        case 3:
          orderSelector = m => m.PostNazMjesta;
          break;
        case 4:
          orderSelector = m => m.OznDrzaveNavigation.NazDrzave;
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
