using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MVC.Models;
using System.Linq;

namespace MVC.IntegrationTest
{
  //npr. ako želimo npr. integracijsko testiranje s in-memory bazom podataka
  //detaljnije na https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-5.0
  public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
  {
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
      builder.ConfigureServices(services =>
      {
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType ==
                typeof(DbContextOptions<FirmaContext>));

        services.Remove(descriptor);

        services.AddDbContext<FirmaContext>(options =>
        {
          options.UseInMemoryDatabase("InMemoryDbForTesting");
        });
      });
    }
  }
}
