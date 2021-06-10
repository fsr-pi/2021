using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC.IntegrationTest
{
  public static class DbInitExtensions
  {
    public static WebApplicationFactory<T> Prepare<T>(this WebApplicationFactory<T> factory, Action<FirmaContext> addDataAction) where T : class
    {
      return factory
        .WithWebHostBuilder(builder =>
        {          
          builder.ConfigureServices(services =>
          {
            var serviceProvider = services.BuildServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
              var scopedServices = scope.ServiceProvider;
              var db = scopedServices.GetRequiredService<FirmaContext>();
              addDataAction(db);
            }
          });
        });
    }
  }
}
