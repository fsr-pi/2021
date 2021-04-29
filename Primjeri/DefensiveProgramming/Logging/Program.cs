using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.IO;
using System.Linq;

namespace Logging
{
  class Program
  {
    static void Main(string[] args)
    {
      using (var serviceProvider = BuildDI())
      {
        var dataLoader = serviceProvider.GetRequiredService<IDataLoader>();
        var list = dataLoader.LoadData("data.txt");
        var sortQuery = list.OrderByDescending(t => t.Item2)
                            .ThenBy(t => t.Item1);

        Console.WriteLine("Valid data: ");
        foreach (var item in sortQuery)
        {
          Console.WriteLine($"{item.Item2:yyyy-MM-dd} {item.Item1}");
        }
      }        
    }

    private static ServiceProvider BuildDI()
    {
      var configuration = new ConfigurationBuilder()                              
                              .SetBasePath(Directory.GetCurrentDirectory())
                              .AddJsonFile("appsettings.json")
                              .Build();

      IServiceCollection services = new ServiceCollection();
      var provider = services.AddLogging(configure => {
                                  configure.AddConfiguration(configuration.GetSection("Logging"));
                                  configure.AddConsole();
                                  configure.AddNLog();
                              })
                             .AddTransient<IDataLoader, DataLoader>()
                             .BuildServiceProvider();
      return provider;
    }
  }
}
