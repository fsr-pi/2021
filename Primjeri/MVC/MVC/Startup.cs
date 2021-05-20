using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MVC.Models;
using System;

namespace MVC
{
  public class Startup
  {
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
      var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
      bool useSecretFile = "Development".Equals(environmentName, StringComparison.OrdinalIgnoreCase);
      

      var appSection = Configuration.GetSection("AppSettings");
      services.Configure<AppSettings>(appSection);

      services.AddDbContext<FirmaContext>(options =>
      {
        string connectionString = Configuration.GetConnectionString("Firma");
        if (useSecretFile)
        {
          connectionString = connectionString.Replace("sifra", Configuration["FirmaSqlPassword"]);
        }
        options.UseSqlServer(connectionString);
      });

      services.AddControllersWithViews()
              .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());              
    }

    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-5.0
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseStaticFiles();
      app.UseRouting();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute("Mjesta i artikli",
              "{action}/{controller:regex(^(Mjesto|Artikl)$)}/Page{page}/Sort{sort:int}/ASC-{ascending:bool}/{id?}",
              new { action = "Index"}
              );

        endpoints.MapDefaultControllerRoute();               
      });     
    }
  }
}
