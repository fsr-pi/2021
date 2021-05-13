using EF.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace EF
{
  class Program
  {
    const int productCode = 12345678;
    static ServiceProvider serviceProvider;
    static void Main(string[] args)
    {
      using (serviceProvider = BuildDI())
      {
        AddProduct();
        ChangeProductPrice();
        DeleteProduct();
        PrintMostExpensives();
        PrintMostExpensivesAnonymous();
      }
    }

    private static ServiceProvider BuildDI()
    {
      var configuration = new ConfigurationBuilder()
                              .AddJsonFile("appsettings.json")                              
                              .Build();

      IServiceCollection services = new ServiceCollection();

      var provider = services.AddDbContext<FirmaContext>(options => {
                                      options.UseSqlServer(configuration.GetConnectionString("Firma"));
                                    },
                                    contextLifetime: ServiceLifetime.Transient
                              )
                             .BuildServiceProvider();
      return provider;
    }


    private static void AddProduct()
    {
      try
      {
        using (var context = serviceProvider.GetService<FirmaContext>())
        {
          Artikl artikl = new Artikl()
          {
            SifArtikla = productCode,
            CijArtikla = 500m,
            JedMjere = "kom",
            NazArtikla = "Cheap Phone v1"
          };
          context.Artikl.Add(artikl);  //context.Set<Artikl>().Add(artikl);
          context.SaveChanges();
          Console.WriteLine($"Product #{artikl.SifArtikla} successfully added");
        }
      }
      catch (Exception exc)
      {
        Console.WriteLine($"Error adding product #{productCode}: {exc.CompleteExceptionMessage()}");
      }
    }

    private static void DeleteProduct()
    {
      try
      {
        using (var context = serviceProvider.GetService<FirmaContext>())
        {
          //Artikl artikl = context.Find<Artikl>(sifraArtikla);
          Artikl artikl = context.Artikl.Find(productCode);
          context.Artikl.Remove(artikl);
          //context.Entry(artikl).State = EntityState.Deleted;
          context.SaveChanges();
          Console.WriteLine($"Product  #{artikl.SifArtikla} deleted");
        }
      }
      catch (Exception exc)
      {
        Console.WriteLine($"Error deleting product #{productCode}: {exc.CompleteExceptionMessage()}");
      }
    }

    private static void ChangeProductPrice()
    {
      try
      {
        using (var context = serviceProvider.GetService<FirmaContext>())
        {
          Artikl artikl = context.Artikl.Find(productCode);
          artikl.CijArtikla = 600m;
          context.SaveChanges();
          Console.WriteLine("Price changed");
        }
      }
      catch (Exception exc)
      {
        Console.WriteLine($"Error trying to change product #{productCode} price: {exc.CompleteExceptionMessage()}");
      }
    }


    private static void PrintMostExpensives()
    {
      Console.WriteLine("How many most expensive products to print?");
      int n = int.Parse(Console.ReadLine());
      using (var context = serviceProvider.GetService<FirmaContext>())
      {
        var query = context.Artikl
                           .Include(a => a.Stavka)
                           .AsNoTracking()
                           .OrderByDescending(a => a.CijArtikla)
                           .Take(n);
        int cnt = 0;
        foreach (var product in query)
        {
          Console.WriteLine("{0}. {1} - {2:C2} ({3})", ++cnt, product.NazArtikla, product.CijArtikla, product.Stavka.Count);
        }
      }
    }

    private static void PrintMostExpensivesAnonymous()
    {
      Console.WriteLine("How many most expensive products to print?");
      int n = int.Parse(Console.ReadLine());
      using (var context = serviceProvider.GetService<FirmaContext>())
      {
        var query = context.Artikl
                           .OrderByDescending(a => a.CijArtikla)
                           .Select(a => new { a.NazArtikla, a.CijArtikla, Sales = a.Stavka.Count })
                           .Take(n);
        int cnt = 0;
        foreach (var product in query)
        {
          Console.WriteLine("{0}. {1} - {2:C2} ({3})", ++cnt, product.NazArtikla, product.CijArtikla, product.Sales);
        }
      }
    }
  }
}