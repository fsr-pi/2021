using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
  public class MjestoController : Controller
  {
    private readonly FirmaContext ctx;
    private readonly AppSettings appData;
   
    public MjestoController(FirmaContext ctx, IOptionsSnapshot<AppSettings> options)
    {
      this.ctx = ctx;
      appData = options.Value;
    }

    public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
    {
      int pagesize = appData.PageSize;

      var query = ctx.Mjesto.AsNoTracking();
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
     
      var mjesta = await query
                          .Select(m => new MjestoViewModel
                          {
                            IdMjesta = m.IdMjesta,
                            NazivMjesta = m.NazMjesta,
                            PostBrojMjesta = m.PostBrMjesta,
                            PostNazivMjesta = m.PostNazMjesta,
                            NazivDrzave = m.OznDrzaveNavigation.NazDrzave
                          })
                          .Skip((page - 1) * pagesize)
                          .Take(pagesize)
                          .ToListAsync();
      var model = new MjestaViewModel
      {
        Mjesta = mjesta,
        PagingInfo = pagingInfo
      };

      return View(model);
    }   

    [HttpGet]
    public async Task<IActionResult> Create()
    {
      await PrepareDropDownLists();
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Mjesto mjesto)
    {
      if (ModelState.IsValid)
      {
        try
        {
          ctx.Add(mjesto);
          await ctx.SaveChangesAsync();

          TempData[Constants.Message] = $"Mjesto {mjesto.NazMjesta} dodano. Id mjesta = {mjesto.IdMjesta}";
          TempData[Constants.ErrorOccurred] = false;
          return RedirectToAction(nameof(Index));

        }
        catch (Exception exc)
        {
          ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
          await PrepareDropDownLists();
          return View(mjesto);
        }
      }
      else
      {
        await PrepareDropDownLists();
        return View(mjesto);
      }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int page = 1, int sort = 1, bool ascending = true)
    {
      var mjesto = await ctx.Mjesto.FindAsync(id);                       
      if (mjesto != null)
      {
        try
        {
          string naziv = mjesto.NazMjesta;
          ctx.Remove(mjesto);          
          await ctx.SaveChangesAsync();
          TempData[Constants.Message] = $"Mjesto {naziv} sa šifrom {id} obrisano.";
          TempData[Constants.ErrorOccurred] = false;        
        }
        catch (Exception exc)
        {
          TempData[Constants.Message] = "Pogreška prilikom brisanja mjesta: " + exc.CompleteExceptionMessage();
          TempData[Constants.ErrorOccurred] = true;         
        }
      }
      else
      {
        TempData[Constants.Message] = $"Ne postoji mjesto sa šifrom: {id}";
        TempData[Constants.ErrorOccurred] = true;
      }
      return RedirectToAction(nameof(Index), new { page, sort, ascending });
    }

    private async Task PrepareDropDownLists()
    {
      var hr = await ctx.Drzava                  
                        .Where(d => d.OznDrzave == "HR")
                        .Select(d => new { d.NazDrzave, d.OznDrzave })
                        .FirstOrDefaultAsync();
      var drzave = await ctx.Drzava                      
                            .Where(d => d.OznDrzave != "HR")
                            .OrderBy(d => d.NazDrzave)
                            .Select(d => new { d.NazDrzave, d.OznDrzave })
                            .ToListAsync();
      if (hr != null)
      {
        drzave.Insert(0, hr);
      }      
      ViewBag.Drzave = new SelectList(drzave, nameof(Drzava.OznDrzave), nameof(Drzava.NazDrzave));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, int page = 1, int sort = 1, bool ascending = true)
    {
      var mjesto = await ctx.Mjesto
                            .AsNoTracking()
                            .Where(m => m.IdMjesta == id)
                            .SingleOrDefaultAsync();
      if (mjesto != null)
      {
        ViewBag.Page = page;
        ViewBag.Sort = sort;
        ViewBag.Ascending = ascending;
        await PrepareDropDownLists();
        return View(mjesto);
      }
      else
      {
        return NotFound($"Neispravan id mjesta: {id}");
      }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Mjesto mjesto, int page = 1, int sort = 1, bool ascending = true)
    {
      if (mjesto == null)
      {
        return NotFound("Nema poslanih podataka");
      }
      bool checkId = await ctx.Mjesto.AnyAsync(m => m.IdMjesta == mjesto.IdMjesta);
      if (!checkId)
      {
        return NotFound($"Neispravan id mjesta: {mjesto?.IdMjesta}");
      }
      
      if (ModelState.IsValid)
      {
        try
        {
          ctx.Update(mjesto);
          await ctx.SaveChangesAsync();
          TempData[Constants.Message] = "Mjesto ažurirano.";
          TempData[Constants.ErrorOccurred] = false;
          return RedirectToAction(nameof(Index), new { page, sort, ascending });
        }
        catch (Exception exc)
        {
          ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
          await PrepareDropDownLists();
          return View(mjesto);
        }
      }
      else
      {
        await PrepareDropDownLists();
        return View(mjesto);
      }
    }

    #region Metode za dinamičko brisanje i ažuriranje
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAjax(int id)
    {
      var mjesto = await ctx.Mjesto.FindAsync(id);                      
      if (mjesto != null)
      {
        try
        {
          string naziv = mjesto.NazMjesta;
          ctx.Remove(mjesto);
          await ctx.SaveChangesAsync();
          var result = new
          {
            message = $"Mjesto {naziv} sa šifrom {id} obrisano.",
            successful = true
          };
          return Json(result);
        }
        catch (Exception exc)
        {
          var result = new
          {
            message = "Pogreška prilikom brisanja mjesta: " + exc.CompleteExceptionMessage(),
            successful = false
          };
          return Json(result);
        }
      }
      else
      {
        return NotFound($"Mjesto sa šifrom {id} ne postoji");
      }
    }

    [HttpGet]
    public async Task<IActionResult> EditAjax(int id)
    {
      var mjesto = await ctx.Mjesto
                            .AsNoTracking()
                            .Where(m => m.IdMjesta == id)
                            .SingleOrDefaultAsync();
      if (mjesto != null)
      {        
        await PrepareDropDownLists();
        return PartialView(mjesto);
      }
      else
      {
        return NotFound($"Neispravan id mjesta: {id}");
      }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAjax(int id, Mjesto mjesto)
    {
      if (mjesto == null)
      {
        return NotFound("Nema poslanih podataka");
      }
      bool checkId = await ctx.Mjesto.AnyAsync(m => m.IdMjesta == mjesto.IdMjesta);
      if (!checkId)
      {
        return NotFound($"Neispravan id mjesta: {mjesto?.IdMjesta}");
      }

      if (ModelState.IsValid)
      {
        try
        {
          ctx.Update(mjesto);
          await ctx.SaveChangesAsync();
          return NoContent();
        }
        catch (Exception exc)
        {
          ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
          await PrepareDropDownLists();
          return PartialView(mjesto);
        }
      }
      else
      {
        await PrepareDropDownLists();
        return PartialView(mjesto);
      }
    }

    [HttpGet]
    public async Task<IActionResult> GetAjax(int id)
    {
      var mjesto = await ctx.Mjesto                            
                            .Where(m => m.IdMjesta == id)
                            .Select(m => new MjestoViewModel
                            {
                              IdMjesta = m.IdMjesta,
                              NazivMjesta = m.NazMjesta,
                              PostBrojMjesta = m.PostBrMjesta,
                              PostNazivMjesta = m.PostNazMjesta,
                              NazivDrzave = m.OznDrzaveNavigation.NazDrzave
                            })
                            .SingleOrDefaultAsync();
      if (mjesto != null)
      {
        return PartialView(mjesto);
      }
      else
      {
        return NotFound($"Neispravan id mjesta: {id}");
      }
    }
    #endregion
  }
}
