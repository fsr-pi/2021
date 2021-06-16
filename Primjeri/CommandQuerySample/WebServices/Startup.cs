using AutoMapper;
using CommandQueryCore;
using Contract.CommandHandlers;
using Contract.Commands;
using Contract.Queries;
using Contract.QueryHandlers;
using DAL.CommandHandlers;
using DAL.Models;
using DAL.QueryHandlers;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using WebServices.Controllers;

namespace WebServices
{
  public class Startup
  {
    public IConfiguration Configuration { get; }

    public IWebHostEnvironment CurrentEnvironment { get; }

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
      Configuration = configuration;
      CurrentEnvironment = env;
    }

    public void ConfigureServices(IServiceCollection services)
    {
      bool useSecretFile = CurrentEnvironment.IsDevelopment();     
      services.AddDbContext<FirmaContext>(options =>
      {
        string connectionString = Configuration.GetConnectionString("Firma");
        if (useSecretFile)
        {
          connectionString = connectionString.Replace("sifra", Configuration["FirmaSqlPassword"]);
        }
        options.UseSqlServer(connectionString);
      });
      
      services.AddControllers()
              .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Contract.DTOs.MjestoValidator>())
              .AddJsonOptions(configure => configure.JsonSerializerOptions.PropertyNamingPolicy = null);

      services.AddSwaggerGen(c =>
      {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
      });

      services.AddTransient<MjestoController>();

      services.AddAutoMapper(typeof(Startup), typeof(Util.ApiModelsMappingProfile));
      #region Register handlers
      services.AddTransient<IMjestaQueryHandler, MjestaQueryHandler>();
      services.AddTransient<IMjestoQueryHandler, MjestoQueryHandler>();
      services.AddTransient<IMjestoCountQueryHandler, MjestoCountQueryHandler>();

      services.AddTransient<IQueryHandler<SearchMjestoQuery, IEnumerable<Contract.DTOs.Mjesto>>, SearchMjestoQueryHandler>();
            
      services.AddTransient<ICommandHandler<DeleteMjesto>, MjestoCommandHandler>();

      services.AddTransient<MjestoCommandHandler>();
      services.AddTransient<ICommandHandler<AddMjesto, int>, ValidateCommandBeforeHandle<AddMjesto, int, MjestoCommandHandler>>();
      services.AddTransient<ICommandHandler<UpdateMjesto>, ValidateCommandBeforeHandle<UpdateMjesto, MjestoCommandHandler>>();

      services.AddTransient<IDrzaveLookupQueryHandler, DrzaveLookupQueryHandler>();
      #endregion
    }
    

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("swagger/v1/swagger.json", "Firma WebAPI");
        c.RoutePrefix = string.Empty;
      });

      app.UseStaticFiles();
      app.UseRouting();

      app.UseEndpoints(endpoints =>
      {        
        endpoints.MapControllers();        
      });
    }
  }
}
