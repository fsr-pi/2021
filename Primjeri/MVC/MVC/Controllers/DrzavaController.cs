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
using System.Text.Json;
using System.Threading.Tasks;

namespace MVC.Controllers
{
  public class DrzavaController : Controller
  {
    private readonly FirmaContext ctx;
    private readonly ILogger<DrzavaController> logger;
    private readonly AppSettings appSettings;

    public DrzavaController(FirmaContext ctx, IOptionsSnapshot<AppSettings> options, ILogger<DrzavaController> logger)
    {
      this.ctx = ctx;
      this.logger = logger;
      appSettings = options.Value;
    }

    //public IActionResult Index()
    //{
    //  var drzave = ctx.Drzava
    //                  .AsNoTracking()
    //                  .OrderBy(d => d.NazDrzave)
    //                  .ToList();
    //  return View("IndexSimple", drzave);
    //}

    public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
    {
      int pagesize = appSettings.PageSize;

      var query = ctx.Drzava
                     .AsNoTracking();

      int count = query.Count();
      if (count == 0)
      {
        logger.LogInformation("Ne postoji nijedna država");
        TempData[Constants.Message] = "Ne postoji niti jedna država.";
        TempData[Constants.ErrorOccurred] = false;
        return RedirectToAction(nameof(Create));
      }

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
        return RedirectToAction(nameof(Index), new { page = 1, sort, ascending });
      }

      query = query.ApplySort(sort, ascending);
      
      var drzave = query
                  .Skip((page - 1) * pagesize)
                  .Take(pagesize)
                  .ToList();

      var model = new DrzaveViewModel
      {
        Drzave = drzave,
        PagingInfo = pagingInfo
      };

      return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Drzava drzava)
    {
      logger.LogTrace(JsonSerializer.Serialize(drzava));
      if (ModelState.IsValid)
      {
        try
        {
          ctx.Add(drzava);
          ctx.SaveChanges();
          logger.LogInformation(new EventId(1000), $"Država {drzava.NazDrzave} dodana.");

          TempData[Constants.Message] = $"Država {drzava.NazDrzave} dodana.";
          TempData[Constants.ErrorOccurred] = false;
          return RedirectToAction(nameof(Index));
        }
        catch (Exception exc)
        {
          logger.LogError("Pogreška prilikom dodavanje nove države: {0}", exc.CompleteExceptionMessage());
          ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
          return View(drzava);
        }
      }
      else
      {
        return View(drzava);
      }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(string OznDrzave, int page = 1, int sort = 1, bool ascending = true)
    {
      var drzava = ctx.Drzava.Find(OznDrzave);                       
      if (drzava != null)
      {
        try
        {
          string naziv = drzava.NazDrzave;
          ctx.Remove(drzava);
          ctx.SaveChanges();
          logger.LogInformation($"Država {naziv} uspješno obrisana");
          TempData[Constants.Message] = $"Država {naziv} uspješno obrisana";
          TempData[Constants.ErrorOccurred] = false;
        }
        catch (Exception exc)
        {
          TempData[Constants.Message] = "Pogreška prilikom brisanja države: " + exc.CompleteExceptionMessage();
          TempData[Constants.ErrorOccurred] = true;
          logger.LogError("Pogreška prilikom brisanja države: " + exc.CompleteExceptionMessage());
        }
      }
      else
      {
        logger.LogWarning("Ne postoji država s oznakom: {0} ", OznDrzave);
        TempData[Constants.Message] = "Ne postoji država s oznakom: " + OznDrzave;
        TempData[Constants.ErrorOccurred] = true;
      }
      return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
    }

    [HttpGet]
    public IActionResult Edit(string id, int page = 1, int sort = 1, bool ascending = true)
    {
      var drzava = ctx.Drzava.AsNoTracking().Where(d => d.OznDrzave == id).SingleOrDefault();
      if (drzava == null)
      {
        logger.LogWarning("Ne postoji država s oznakom: {0} ", id);
        return NotFound("Ne postoji država s oznakom: " + id);
      }
      else
      {
        ViewBag.Page = page;
        ViewBag.Sort = sort;
        ViewBag.Ascending = ascending;
        return View(drzava);
      }
    }

    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(string id, int page = 1, int sort = 1, bool ascending = true)
    {
      //za različite mogućnosti ažuriranja pogledati
      //attach, update, samo id, ...
      //https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud#update-the-edit-page

      try
      {
        Drzava drzava = await ctx.Drzava
                          .Where(d => d.OznDrzave == id)
                          .FirstOrDefaultAsync();
        if (drzava == null)
        {
          return NotFound("Neispravna oznaka države: " + id);
        }

        if (await TryUpdateModelAsync<Drzava>(drzava, "",
            d => d.NazDrzave, d => d.SifDrzave, d => d.Iso3drzave
        ))
        {
          ViewBag.Page = page;
          ViewBag.Sort = sort;
          ViewBag.Ascending = ascending;
          try
          {
            await ctx.SaveChangesAsync();
            TempData[Constants.Message] = "Država ažurirana.";
            TempData[Constants.ErrorOccurred] = false;
            return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
          }
          catch (Exception exc)
          {
            ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
            return View(drzava);
          }
        }
        else
        {
          ModelState.AddModelError(string.Empty, "Podatke o državi nije moguće povezati s forme");
          return View(drzava);
        }
      }
      catch (Exception exc)
      {
        TempData[Constants.Message] = exc.CompleteExceptionMessage();
        TempData[Constants.ErrorOccurred] = true;
        return RedirectToAction(nameof(Edit), id);
      }
    }
  }
}
