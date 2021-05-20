using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using MVC.Extensions;
using MVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using MVC.ViewModels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MVC.Extensions.Selectors;
using MVC.Util;

namespace MVC.Controllers
{
  public class ArtiklController : Controller
  {
    private readonly FirmaContext ctx;
    private readonly ILogger<ArtiklController> logger;
    private readonly AppSettings appData;

    public ArtiklController(FirmaContext ctx, IOptionsSnapshot<AppSettings> options, ILogger<ArtiklController> logger)
    {
      this.ctx = ctx;
      this.logger = logger;
      appData = options.Value;
    }

    public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
    {
      int pagesize = appData.PageSize;
      var query = ctx.Artikl.AsNoTracking(); //or AsQueryable()
      int count = await query.CountAsync();

      var pagingInfo = new PagingInfo
      {
        CurrentPage = page,
        Sort = sort,
        Ascending = ascending,
        ItemsPerPage = pagesize,
        TotalItems = count
      };
      if (page < 1 || page > pagingInfo.TotalPages) {       
        return RedirectToAction(nameof(Index), new { page = 1, sort, ascending });
      }

      query = query.ApplySort(sort, ascending);      

      var artikli = await query
                          .Select(a => new ArtiklViewModel
                          {
                            SifraArtikla = a.SifArtikla,
                            NazivArtikla = a.NazArtikla,
                            JedinicaMjere = a.JedMjere,
                            CijenaArtikla = a.CijArtikla,
                            Usluga = a.ZastUsluga,
                            TekstArtikla = a.TekstArtikla,
                            ImaSliku = a.SlikaArtikla != null,
                            ImageHash = a.SlikaChecksum
                          })
                          .Skip((page - 1) * pagesize)
                          .Take(pagesize)
                          .ToListAsync();
      var model = new ArtikliViewModel
      {
        Artikli = artikli,
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
    public async Task<IActionResult> Create(Artikl artikl, IFormFile slika)
    {
      //provjeri jedinstvenost šifre artikla
      bool exists = await ctx.Artikl.AnyAsync(a => a.SifArtikla == artikl.SifArtikla);
      if (exists)
      {
        ModelState.AddModelError(nameof(Artikl.SifArtikla), "Artikl s navedenom šifrom već postoji");
      }
      if (ModelState.IsValid)
      {
        try
        {
          if (slika != null && slika.Length > 0)
          {
            
            using (MemoryStream stream = new MemoryStream())
            {
              await slika.CopyToAsync(stream);
              byte[] image = stream.ToArray();              
              artikl.SlikaArtikla = ImageUtil.CreateThumbnail(image, maxheight: appData.ImageSettings.ThumbnailHeight);
            }            
          }
          ctx.Add(artikl);
          await ctx.SaveChangesAsync();

          TempData[Constants.Message] = $"Artikl  {artikl.SifArtikla} - {artikl.NazArtikla} dodan";
          TempData[Constants.ErrorOccurred] = false;
          return RedirectToAction(nameof(Index));

        }
        catch (Exception exc)
        {
          ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
          return View(artikl);
        }
      }
      else
      {
        return View(artikl);
      }
    }    

    public async Task<FileContentResult> GetImage(int id)
    {
      byte[] image = await ctx.Artikl
                              .Where(a => a.SifArtikla == id)
                              .Select(a => a.SlikaArtikla)
                              .SingleOrDefaultAsync();
      if (image != null)
      {        
        return File(image, "image/jpeg");
      }
      else
      {
        return null;
      }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
      var artikl = await ctx.Artikl
                            .Where(a => a.SifArtikla == id)
                            .Select(a => new Artikl
                            {
                              CijArtikla = a.CijArtikla,
                              JedMjere = a.JedMjere,
                              NazArtikla = a.NazArtikla,
                              SifArtikla = a.SifArtikla,
                              TekstArtikla = a.TekstArtikla,
                              ZastUsluga = a.ZastUsluga
                            })
                            .FirstOrDefaultAsync();

      if (artikl != null)
      {
        return PartialView(artikl);
      }
      else
      {
        return NotFound($"Neispravna šifra artikla: {id}");
      }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Artikl artikl, IFormFile slika, bool obrisiSliku)
    {
      if (artikl == null)
      {
        return NotFound("Nema poslanih podataka");
      }
      Artikl dbArtikl = await ctx.Artikl.FindAsync(artikl.SifArtikla);
      if (dbArtikl == null)
      {
        return NotFound($"Neispravna šifra artikla: {artikl.SifArtikla}");
      }
    
      if (ModelState.IsValid)
      {
        try
        {
          //ne možemo ići na varijantu ctx.Update(artikl), jer nismo prenosili sliku, pa bi bila obrisana
          dbArtikl.JedMjere = artikl.JedMjere;
          dbArtikl.NazArtikla = artikl.NazArtikla;
          dbArtikl.ZastUsluga = artikl.ZastUsluga;
          dbArtikl.TekstArtikla = artikl.TekstArtikla;
          dbArtikl.CijArtikla = artikl.CijArtikla;

          if (slika != null && slika.Length > 0)
          {
            using (MemoryStream stream = new MemoryStream())
            {
              slika.CopyTo(stream);
              byte[] image = stream.ToArray();
              image = ImageUtil.CreateThumbnail(image, maxheight: appData.ImageSettings.ThumbnailHeight);
              dbArtikl.SlikaArtikla = image;              
            }
          }
          else if (obrisiSliku)
          {
            dbArtikl.SlikaArtikla = null;
          }

          await ctx.SaveChangesAsync();
          return NoContent();
        }
        catch (Exception exc)
        {
          ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
          return PartialView(artikl);
        }
      }
      else
      {
        return PartialView(artikl);
      }
    }

    public async Task<IActionResult> Get(int id)
    {
      var artikl = await ctx.Artikl
                            .AsNoTracking()
                            .Where(a => a.SifArtikla == id)
                            .Select(a => new ArtiklViewModel
                            {
                              SifraArtikla = a.SifArtikla,
                              NazivArtikla = a.NazArtikla,
                              JedinicaMjere = a.JedMjere,
                              CijenaArtikla = a.CijArtikla,
                              Usluga = a.ZastUsluga,
                              TekstArtikla = a.TekstArtikla,
                              ImaSliku = a.SlikaArtikla != null,
                              ImageHash = a.SlikaChecksum
                            })
                            .SingleOrDefaultAsync();
      if (artikl != null)
      {
        return PartialView(artikl);
      }
      else
      {
        return NotFound($"Neispravna šifra artikla: {id}");
      }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
      var artikl = await ctx.Artikl.FindAsync(id);
      if (artikl != null)
      {
        try
        {
          string naziv = artikl.NazArtikla;
          ctx.Remove(artikl);
          await ctx.SaveChangesAsync();
          var result = new
          {
            message = $"Artikl {naziv} sa šifrom {id} uspješno obrisan.",
            successful = true
          };
          return Json(result);
        }
        catch (Exception exc)
        {
          var result = new
          {
            message = "Pogreška prilikom brisanja artikla: " + exc.CompleteExceptionMessage(),
            successful = false
          };
          return Json(result);
        }
      }
      else
      {
        return NotFound($"Artikl sa šifrom {id} ne postoji");
      }
    }

    public async Task<bool> ProvjeriSifruArtikla(int SifArtikla)
    {
      bool postoji = await ctx.Artikl.AnyAsync(a => a.SifArtikla == SifArtikla);
      return !postoji;
    }
  }
}
