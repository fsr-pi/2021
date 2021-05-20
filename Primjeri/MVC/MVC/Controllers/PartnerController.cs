using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MVC.Extensions;
using MVC.Extensions.Selectors;
using MVC.Models;
using MVC.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Controllers
{
  public class PartnerController : Controller
  {
    private readonly FirmaContext ctx;
    private readonly ILogger<PartnerController> logger;
    private readonly AppSettings appData;

    public PartnerController(FirmaContext ctx, IOptionsSnapshot<AppSettings> options, ILogger<PartnerController> logger)
    {
      this.ctx = ctx;
      this.logger = logger;
      appData = options.Value;
    }

    public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
    {
      int pagesize = appData.PageSize;

      var query = ctx.vw_Partner.AsQueryable();
      int count = await query.CountAsync();

      var pagingInfo = new PagingInfo
      {
        CurrentPage = page,
        Sort = sort,
        Ascending = ascending,
        ItemsPerPage = pagesize,
        TotalItems = count
      };
      if (page < 1 || page > pagingInfo.TotalPages)
      {
        return RedirectToAction(nameof(Index), new { page = 1, sort = sort, ascending = ascending });
      }

      query = query.ApplySort(sort, ascending);

      var partneri = await query
                          .Skip((page - 1) * pagesize)
                          .Take(pagesize)                          
                          .ToListAsync();
      var model = new PartneriViewModel
      {
        Partneri = partneri,
        PagingInfo = pagingInfo
      };

      return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
      PartnerViewModel model = new PartnerViewModel
      {
        TipPartnera = "O"
      };
      return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PartnerViewModel model)
    {
      if (ModelState.IsValid)
      {
        Partner p = new Partner();
        p.TipPartnera = model.TipPartnera;
        CopyValues(p, model);
        try
        {
          ctx.Add(p);
          await ctx.SaveChangesAsync();

          TempData[Constants.Message] = $"Partner uspješno dodan. Id={p.IdPartnera}";
          TempData[Constants.ErrorOccurred] = false;
          return RedirectToAction(nameof(Index));

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
    public async Task<IActionResult> Delete(int id, int page = 1, int sort = 1, bool ascending = true)
    {
      var partner = await ctx.Partner.FindAsync(id);
      if (partner != null)
      {
        try
        {
          ctx.Remove(partner);
          await ctx.SaveChangesAsync();
          TempData[Constants.Message] = $"Partner {partner.IdPartnera} uspješno obrisan.";
          TempData[Constants.ErrorOccurred] = false;
        }
        catch (Exception exc)
        {
          TempData[Constants.Message] = "Pogreška prilikom brisanja partnera: " + exc.CompleteExceptionMessage();
          TempData[Constants.ErrorOccurred] = true;
        }
      }
      else
      {
        TempData[Constants.Message] = "Ne postoji partner s id-om: " + id;
        TempData[Constants.ErrorOccurred] = true;
      }
      return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, int page = 1, int sort = 1, bool ascending = true)
    {
      var partner = await ctx.Partner.FindAsync(id);
      if (partner == null)
      {
        return NotFound("Ne postoji partner s oznakom: " + id);
      }
      else
      {
        PartnerViewModel model = new PartnerViewModel
        {
          IdPartnera = partner.IdPartnera,
          IdMjestaIsporuke = partner.IdMjestaIsporuke,
          IdMjestaPartnera = partner.IdMjestaPartnera,
          AdrIsporuke = partner.AdrIsporuke,
          AdrPartnera = partner.AdrPartnera,
          Oib = partner.Oib,
          TipPartnera = partner.TipPartnera
        };
        if (model.TipPartnera == "O")
        {
          Osoba osoba = ctx.Osoba.AsNoTracking()
                           .Where(o => o.IdOsobe == model.IdPartnera)
                           .First(); //Single()
          model.ImeOsobe = osoba.ImeOsobe;
          model.PrezimeOsobe = osoba.PrezimeOsobe;
        }
        else
        {
          Tvrtka tvrtka = ctx.Tvrtka.AsNoTracking()
                           .Where(t => t.IdTvrtke == model.IdPartnera)
                           .First(); //Single()
          model.MatBrTvrtke = tvrtka.MatBrTvrtke;
          model.NazivTvrtke = tvrtka.NazivTvrtke;
        }

        await DohvatiNaziveMjesta(model);

        ViewBag.Page = page;
        ViewBag.Sort = sort;
        ViewBag.Ascending = ascending;
        return View(model);
      }
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(PartnerViewModel model, int page = 1, int sort = 1, bool ascending = true)
    {
      if (model == null)
      {
        return NotFound("Nema poslanih podataka");
      }
      var partner = await ctx.Partner.FindAsync(model.IdPartnera);
      if (partner == null)
      {
        return NotFound("Ne postoji partner s id-om: " + model.IdPartnera);
      }

      ViewBag.Page = page;
      ViewBag.Sort = sort;
      ViewBag.Ascending = ascending;

      if (ModelState.IsValid)
      {
        try
        {
          CopyValues(partner, model);

          //vezani dio je stvoren s new Osoba() ili new Tvrtka() pa je entity stated Added što bi proizvelo Insert pa ne update
          if (partner.Osoba != null)
          {
            partner.Osoba.IdOsobe = partner.IdPartnera;
            ctx.Entry(partner.Osoba).State = EntityState.Modified;
          }
          if (partner.Tvrtka != null)
          {
            partner.Tvrtka.IdTvrtke = partner.IdPartnera;
            ctx.Entry(partner.Tvrtka).State = EntityState.Modified;
          }

          await ctx.SaveChangesAsync();
          TempData[Constants.Message] = $"Partner {model.IdPartnera} uspješno ažuriran";
          TempData[Constants.ErrorOccurred] = false;
          return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });

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


    #region Private methods
    private void CopyValues(Partner partner, PartnerViewModel model)
    {
      partner.AdrIsporuke = model.AdrIsporuke;
      partner.AdrPartnera = model.AdrPartnera;
      partner.IdMjestaIsporuke = model.IdMjestaIsporuke;
      partner.IdMjestaPartnera = model.IdMjestaPartnera;
      partner.Oib = model.Oib;
      if (partner.TipPartnera == "O")
      {
        partner.Osoba = new Osoba();
        partner.Osoba.ImeOsobe = model.ImeOsobe;
        partner.Osoba.PrezimeOsobe = model.PrezimeOsobe;
      }
      else
      {
        partner.Tvrtka = new Tvrtka();
        partner.Tvrtka.MatBrTvrtke = model.MatBrTvrtke;
        partner.Tvrtka.NazivTvrtke = model.NazivTvrtke;
      }

    }

    private async Task DohvatiNaziveMjesta(PartnerViewModel model)
    {
      try
      {
        //dohvati nazive mjesta                
        if (model.IdMjestaPartnera.HasValue)
        {
          var mjesto = await ctx.Mjesto
                                .Where(m => m.IdMjesta == model.IdMjestaPartnera.Value)
                                .Select(m => new { m.PostBrMjesta, m.NazMjesta })
                                .FirstAsync();
          model.NazMjestaPartnera = string.Format("{0} {1}", mjesto.PostBrMjesta, mjesto.NazMjesta);
        }
        if (model.IdMjestaIsporuke.HasValue)
        {
          var mjesto = await ctx.Mjesto
                                .Where(m => m.IdMjesta == model.IdMjestaIsporuke.Value)
                                .Select(m => new { m.PostBrMjesta, m.NazMjesta })
                                .FirstAsync();
          model.NazMjestaIsporuke = string.Format("{0} {1}", mjesto.PostBrMjesta, mjesto.NazMjesta);
        }
      }
      catch (Exception exc)
      {
        logger.LogWarning(exc, "Pogreška prilikom dohvata naziva mjesta");        
      }
    }

    #endregion
  }
}
