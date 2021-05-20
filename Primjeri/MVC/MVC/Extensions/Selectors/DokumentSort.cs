using MVC.Models;
using System;
using System.Linq;

namespace MVC.Extensions.Selectors
{
  public static class DokumentSort
  {
    public static IQueryable<ViewDokumentInfo> ApplySort(this IQueryable<ViewDokumentInfo> query, int sort, bool ascending)
    {
      System.Linq.Expressions.Expression<Func<ViewDokumentInfo, object>> orderSelector = null;
      switch (sort)
      {
        case 1:
          orderSelector = d => d.IdDokumenta;
          break;
        case 2:
          orderSelector = d => d.NazPartnera;
          break;
        case 3:
          orderSelector = d => d.DatDokumenta;
          break;
        case 4:
          orderSelector = d => d.IznosDokumenta;
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
