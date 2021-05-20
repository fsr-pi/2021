using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using MVC.ViewModels;
using MVC.Extensions;
using MVC.Extensions.Selectors;

namespace MVC.Controllers
{
  public class DokumentController : Controller
  {
    private readonly FirmaContext ctx;
    private readonly AppSettings appData;

    public DokumentController(FirmaContext ctx, IOptionsSnapshot<AppSettings> options)
    {
      this.ctx = ctx;
      appData = options.Value;
    }

    public async Task<IActionResult> Index(string filter, int page = 1, int sort = 1, bool ascending = true)
    {
      int pagesize = appData.PageSize;
      var query = ctx.vw_Dokumenti.AsQueryable();

      #region Apply filter
      DokumentFilter df = DokumentFilter.FromString(filter);
      if (!df.IsEmpty())
      {
        if (df.IdPartnera.HasValue)
        {
          df.NazPartnera = await ctx.vw_Partner
                                    .Where(p => p.IdPartnera == df.IdPartnera)
                                    .Select(vp => vp.Naziv)
                                    .FirstOrDefaultAsync();
        }
        query = df.Apply(query);
      }
      #endregion

      int count = await query.CountAsync();
    
      var pagingInfo = new PagingInfo
      {
        CurrentPage = page,
        Sort = sort,
        Ascending = ascending,
        ItemsPerPage = pagesize,
        TotalItems = count
      };
      
      if (count > 0 && (page < 1 || page > pagingInfo.TotalPages))
      {        
        return RedirectToAction(nameof(Index), new { page = 1, sort, ascending, filter });                
      }

      query = query.ApplySort(sort, ascending);

      var dokumenti = await query
                            .Skip((page - 1) * pagesize)
                            .Take(pagesize)
                            .ToListAsync();

      for (int i = 0; i < dokumenti.Count; i++)
      {
        dokumenti[i].Position = (page - 1) * pagesize + i;
      }
      var model = new DokumentiViewModel
      {
        Dokumenti = dokumenti,
        PagingInfo = pagingInfo,
        Filter = df
      };

      return View(model);
    }



    [HttpPost]
    public IActionResult Filter(DokumentFilter filter)
    {
      return RedirectToAction(nameof(Index), new { filter = filter.ToString() });
    }

    public async Task<IActionResult> Show(int id, int? position, string filter, int page = 1, int sort = 1, bool ascending = true, string viewName = nameof(Show))
    {      
      var dokument = await ctx.Dokument
                              .Where(d => d.IdDokumenta == id)
                              .Select(d => new DokumentViewModel
                              {
                                BrDokumenta = d.BrDokumenta,
                                DatDokumenta = d.DatDokumenta,
                                IdDokumenta = d.IdDokumenta,
                                IdPartnera = d.IdPartnera,
                                IdPrethDokumenta = d.IdPrethDokumenta,
                                IznosDokumenta = d.IznosDokumenta,
                                PostoPorez = d.PostoPorez,
                                VrDokumenta = d.VrDokumenta
                              })
                              .FirstOrDefaultAsync();
      if (dokument == null)
      {
        return NotFound($"Dokument {id} ne postoji");
      }
      else
      {        
        dokument.NazPartnera = await ctx.vw_Partner
                                        .Where(p => p.IdPartnera == dokument.IdPartnera)
                                        .Select(p => p.Naziv)
                                        .FirstOrDefaultAsync();

        if (dokument.IdPrethDokumenta.HasValue)
        {
          dokument.NazPrethodnogDokumenta = await ctx.vw_Dokumenti                                           
                                                     .Where(d => d.IdDokumenta == dokument.IdPrethDokumenta)
                                                     .Select(d => d.IdDokumenta + " " + d.NazPartnera + " " + d.IznosDokumenta)
                                                     .FirstOrDefaultAsync();
        }
        //učitavanje stavki
        var stavke = await ctx.Stavka
                              .Where(s => s.IdDokumenta == dokument.IdDokumenta)
                              .OrderBy(s => s.IdStavke)
                              .Select(s => new StavkaViewModel
                              {
                                IdStavke = s.IdStavke,
                                JedCijArtikla = s.JedCijArtikla,
                                KolArtikla = s.KolArtikla,
                                NazArtikla = s.SifArtiklaNavigation.NazArtikla,
                                PostoRabat = s.PostoRabat,
                                SifArtikla = s.SifArtikla
                              })
                              .ToListAsync();
        dokument.Stavke = stavke;

        if (position.HasValue)
        {
          page = 1 + position.Value / appData.PageSize;
          await SetPreviousAndNext(position.Value, filter, sort, ascending);
        }

        ViewBag.Page = page;
        ViewBag.Sort = sort;
        ViewBag.Ascending = ascending;
        ViewBag.Filter = filter;
        ViewBag.Position = position;

        return View(viewName, dokument);
      }
    }   

    private async Task SetPreviousAndNext(int position, string filter, int sort, bool ascending)
    {
      var query = ctx.vw_Dokumenti.AsQueryable();                     

      DokumentFilter df = new DokumentFilter();
      if (!string.IsNullOrWhiteSpace(filter))
      {
        df = DokumentFilter.FromString(filter);
        if (!df.IsEmpty())
        {
          query = df.Apply(query);
        }
      }

      query = query.ApplySort(sort, ascending);      
      if (position > 0)
      {
        ViewBag.Previous = await query.Skip(position - 1).Select(d => d.IdDokumenta).FirstAsync();
      }
      if (position < await query.CountAsync() - 1) 
      {
        ViewBag.Next = await query.Skip(position + 1).Select(d => d.IdDokumenta).FirstAsync();
      }
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
      int maxbr = await ctx.Dokument.MaxAsync(d => d.BrDokumenta) + 1; //samo za primjer, inače u stvarnosti može biti paralelnih korisnika
      var dokument = new DokumentViewModel
      {
        DatDokumenta = DateTime.Now,
        BrDokumenta = maxbr
      };
      return View(dokument);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DokumentViewModel model)
    {
      if (ModelState.IsValid)
      {
        Dokument d = new Dokument();
        d.BrDokumenta = model.BrDokumenta;
        d.DatDokumenta = model.DatDokumenta.Date;
        d.IdPartnera = model.IdPartnera.Value;
        d.IdPrethDokumenta = model.IdPrethDokumenta;
        d.PostoPorez = model.PostoPorez;
        d.VrDokumenta = model.VrDokumenta;
        foreach (var stavka in model.Stavke)
        {
          Stavka novaStavka = new Stavka();
          novaStavka.SifArtikla = stavka.SifArtikla;
          novaStavka.KolArtikla = stavka.KolArtikla;
          novaStavka.PostoRabat = stavka.PostoRabat;
          novaStavka.JedCijArtikla = stavka.JedCijArtikla;
          d.Stavka.Add(novaStavka);
        }

        d.IznosDokumenta = (1 + d.PostoPorez)
                            * d.Stavka.Sum(s => s.KolArtikla * (1 - s.PostoRabat) * s.JedCijArtikla);
        //eventualno umanji iznos za dodatni popust za kupca i sl... nešto što bi bilo poslovno pravilo
        try
        {
          ctx.Add(d);
          await ctx.SaveChangesAsync();

          TempData[Constants.Message] = $"Dokument uspješno dodan. Id={d.IdDokumenta}";
          TempData[Constants.ErrorOccurred] = false;
          return RedirectToAction(nameof(Edit), new { id = d.IdDokumenta });

        }
        catch (Exception exc)
        {
          ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
          return View(model);
        }
      }
      else
      {
        return View(model);
      }
    }

    [HttpGet]
    public Task<IActionResult> Edit(int id, int? position, string filter, int page = 1, int sort = 1, bool ascending = true)
    {
      return Show(id, position, filter, page, sort, ascending, viewName: nameof(Edit));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(DokumentViewModel model, int? position, string filter, int page = 1, int sort = 1, bool ascending = true)
    {
      ViewBag.Page = page;
      ViewBag.Sort = sort;
      ViewBag.Ascending = ascending;
      ViewBag.Filter = filter;
      ViewBag.Position = position;
      if (ModelState.IsValid)
      {
        var dokument = await ctx.Dokument
                                .Include(d => d.Stavka)
                                .Where(d => d.IdDokumenta == model.IdDokumenta)
                                .FirstOrDefaultAsync();
        if (dokument == null)
        {
          return NotFound("Ne postoji dokument s id-om: " + model.IdDokumenta);
        }

        if (position.HasValue)
        {
          page = 1 + position.Value / appData.PageSize;
          await SetPreviousAndNext(position.Value, filter, sort, ascending);
        }

        dokument.BrDokumenta = model.BrDokumenta;
        dokument.DatDokumenta = model.DatDokumenta.Date;
        dokument.IdPartnera = model.IdPartnera.Value;
        dokument.IdPrethDokumenta = model.IdPrethDokumenta;
        dokument.PostoPorez = model.StopaPoreza / 100m;
        dokument.VrDokumenta = model.VrDokumenta;

        List<int> idStavki = model.Stavke
                                  .Where(s => s.IdStavke > 0)
                                  .Select(s => s.IdStavke)
                                  .ToList();
        //izbaci sve koje su nisu više u modelu
        ctx.RemoveRange(dokument.Stavka.Where(s => !idStavki.Contains(s.IdStavke)));

        foreach (var stavka in model.Stavke)
        {
          //ažuriraj postojeće i dodaj nove
          Stavka novaStavka; // potpuno nova ili dohvaćena ona koju treba izmijeniti
          if (stavka.IdStavke > 0)
          {
            novaStavka = dokument.Stavka.First(s => s.IdStavke == stavka.IdStavke);
          }
          else
          {
            novaStavka = new Stavka();
            dokument.Stavka.Add(novaStavka);
          }
          novaStavka.SifArtikla = stavka.SifArtikla;
          novaStavka.KolArtikla = stavka.KolArtikla;
          novaStavka.PostoRabat = stavka.PostoRabat;
          novaStavka.JedCijArtikla = stavka.JedCijArtikla;
        }

        dokument.IznosDokumenta = (1 + dokument.PostoPorez) *
                                  model.Stavke.Sum(s => s.KolArtikla * (1 - s.PostoRabat) * s.JedCijArtikla);
        //eventualno umanji iznos za dodatni popust za kupca i sl... nešto što bi bilo poslovno pravilo
        try
        {

          await ctx.SaveChangesAsync();

          TempData[Constants.Message] = $"Dokument {dokument.IdDokumenta} uspješno ažuriran.";
          TempData[Constants.ErrorOccurred] = false;
          return RedirectToAction(nameof(Edit), new
          {
            id = dokument.IdDokumenta,
            position,
            filter,
            page,
            sort,
            ascending
          });

        }
        catch (Exception exc)
        {
          ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
          return View(model);
        }
      }
      else
      {
        return View(model);
      }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int IdDokumenta, string filter, int page = 1, int sort = 1, bool ascending = true)
    {
      var dokument = await ctx.Dokument                             
                              .Where(d => d.IdDokumenta == IdDokumenta)
                              .SingleOrDefaultAsync();
      if (dokument != null)
      {
        try
        {
          ctx.Remove(dokument);
          await ctx.SaveChangesAsync();
          TempData[Constants.Message] = $"Dokument {dokument.IdDokumenta} uspješno obrisan.";
          TempData[Constants.ErrorOccurred] = false;
        }
        catch (Exception exc)
        {
          TempData[Constants.Message] = "Pogreška prilikom brisanja dokumenta: " + exc.CompleteExceptionMessage();
          TempData[Constants.ErrorOccurred] = true;
        }
      }
      else
      {
        TempData[Constants.Message] = "Ne postoji dokument s id-om: " + IdDokumenta;
        TempData[Constants.ErrorOccurred] = true;
      }
      return RedirectToAction(nameof(Index), new { filter, page, sort, ascending });
    }
  }
}
