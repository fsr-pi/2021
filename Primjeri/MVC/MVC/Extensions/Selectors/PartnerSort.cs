using MVC.Models;
using System;
using System.Linq;

namespace MVC.Extensions.Selectors
{
  public static class PartnerSort
  {
    public static IQueryable<ViewPartner> ApplySort(this IQueryable<ViewPartner> query, int sort, bool ascending)
    {
      System.Linq.Expressions.Expression<Func<ViewPartner, object>> orderSelector = null;
      switch (sort)
      {
        case 1:
          orderSelector = p => p.IdPartnera;
          break;
        case 2:
          orderSelector = p => p.TipPartnera;
          break;
        case 3:
          orderSelector = p => p.OIB;
          break;
        case 4:
          orderSelector = p => p.Naziv;
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
