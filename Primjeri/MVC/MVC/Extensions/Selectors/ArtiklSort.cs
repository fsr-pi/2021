using MVC.Models;
using System;
using System.Linq;

namespace MVC.Extensions.Selectors
{
  public static class ArtiklSort
  {
    public static IQueryable<Artikl> ApplySort(this IQueryable<Artikl> query, int sort, bool ascending)
    {
      System.Linq.Expressions.Expression<Func<Artikl, object>> orderSelector = null;
      switch (sort)
      {
        case 1:
          orderSelector = a => a.SlikaArtikla; //ima smisla samo ako se želi na početku ili kraju dobiti sve artikle sa slikom
          break;
        case 2:
          orderSelector = a => a.SifArtikla;
          break;
        case 3:
          orderSelector = a => a.NazArtikla;
          break;
        case 4:
          orderSelector = a => a.JedMjere;
          break;
        case 5:
          orderSelector = a => a.CijArtikla;
          break;
        case 6:
          orderSelector = a => a.ZastUsluga;
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
