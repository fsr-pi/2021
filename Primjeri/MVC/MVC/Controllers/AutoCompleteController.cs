using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MVC.Models;
using MVC.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Controllers
{
  public class AutoCompleteController : Controller
  {
    private readonly FirmaContext ctx;
    private readonly AppSettings appData;

    public AutoCompleteController(FirmaContext ctx, IOptionsSnapshot<AppSettings> options)
    {
      this.ctx = ctx;
      appData = options.Value;
    }

    public async Task<IEnumerable<IdLabel>> Mjesto(string term)
    {
      var query = ctx.Mjesto
                      .Select(m => new IdLabel
                      {
                        Id = m.IdMjesta,
                        Label = m.PostBrMjesta + " " + m.NazMjesta
                      })
                      .Where(l => l.Label.Contains(term));

      var list = await query.OrderBy(l => l.Label)
                            .ThenBy(l => l.Id)
                            .Take(appData.AutoCompleteCount)
                            .ToListAsync();
      return list;
    }

    public async Task<IEnumerable<IdLabel>> Partner(string term)
    {
      var query = ctx.vw_Partner
                      .Select(p => new IdLabel
                      {
                        Id = p.IdPartnera,
                        Label = p.Naziv + " (" + p.OIB + ")"
                      })
                      .Where(l => l.Label.Contains(term));

      var list = await query.OrderBy(l => l.Label)
                            .ThenBy(l => l.Id)
                            .Take(appData.AutoCompleteCount)
                            .ToListAsync();

      //var queryOsobe = ctx.Osoba                                                      
      //                    .Select(o => new IdLabel
      //                    {
      //                        Id = o.IdOsobe,
      //                        Label = o.PrezimeOsobe + ", " + o.ImeOsobe + " (" + o.IdOsobeNavigation.Oib +")"
      //                    })
      //                    .Where(l => l.Label.Contains(term));

      //var queryPartneri = ctx.Tvrtka
      //                        .Select(t => new IdLabel
      //                        {
      //                            Id = t.IdTvrtke,
      //                            Label = t.NazivTvrtke + ", " + " (" + t.IdTvrtkeNavigation.Oib + ")"
      //                        })
      //                        .Where(l => l.Label.Contains(term));
      //var list = queryOsobe.Union(queryPartneri)
      //                     .OrderBy(l => l.Label)
      //                     .ThenBy(l => l.Id)
      //                     .ToList();
      return list;
    }

    public async Task<IEnumerable<AutoCompleteArtikl>> Artikl(string term)
    {
      var query = ctx.Artikl
                     .Where(a => a.NazArtikla.Contains(term))
                     .OrderBy(a => a.NazArtikla)
                     .Select(a => new AutoCompleteArtikl
                     {
                       Id = a.SifArtikla,
                       Label = a.NazArtikla,
                       Cijena = a.CijArtikla
                     });

      var list = await query.OrderBy(l => l.Label)
                            .ThenBy(l => l.Id)
                            .Take(appData.AutoCompleteCount)
                            .ToListAsync();
      return list;
    }

    public async Task<IEnumerable<IdLabel>> Dokument(string term)
    {
      var query = ctx.vw_Dokumenti
                      .Select(p => new IdLabel
                      {
                        Id = p.IdDokumenta,
                        Label = p.IdDokumenta + " " + p.NazPartnera + " " + p.IznosDokumenta
                      })
                      .Where(l => l.Label.Contains(term));

      var list = await query.OrderBy(l => l.Label)
                            .ThenBy(l => l.Id)
                            .Take(appData.AutoCompleteCount)
                            .ToListAsync();
      return list;
    }
  }
}
