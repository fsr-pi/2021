﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MVC.Controllers;
using MVC.Models;
using MVC.ViewModels;
using System;
using System.Linq;
using Xunit;

namespace MVC.Tests
{
  public class DrzavaControllerTests
  {   
    IOptionsSnapshot<AppSettings> options;
    public DrzavaControllerTests()
    {
      //imitacija sučelja IOptionsSnapshot<AppSettings>
      var mockOptions = new Mock<IOptionsSnapshot<AppSettings>>();
      var appSettings = new AppSettings
      {
        PageSize = 10 //ostali argumenti su nebitni za ovu grupu testova
      };
      mockOptions.SetupGet(options => options.Value).Returns(appSettings);
      options = mockOptions.Object;

      //logger se imitira u pojedinom testu, jer svaki put treba biti novi objekt
    }
   
    [Trait("UnitTest", "DrzavaController")]    
    [Fact]
    public void AkoNemaDrzavaPreusmjeriNaCreate()
    {
      // Arrange      
      var mockLogger = new Mock<ILogger<DrzavaController>>();
      
      var dbOptions = new DbContextOptionsBuilder<FirmaContext>()
                .UseInMemoryDatabase(databaseName: "FirmaMemory1")
                .Options;

      using (var context = new FirmaContext(dbOptions))
      {
        var controller = new DrzavaController(context, options, mockLogger.Object);
        var tempDataMock = new Mock<ITempDataDictionary>();                
        controller.TempData = tempDataMock.Object;

        // Act
        var result = controller.Index();

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Create", redirectToActionResult.ActionName);

        mockLogger.Verify(l => l.Log(LogLevel.Information,
                                    It.IsAny<EventId>(),
                                    It.IsAny<object>(),
                                    It.IsAny<Exception>(),
                                    (Func<object, Exception, string>)It.IsAny<object>())
                                 , Times.Once());
      }
    }


    [Trait("UnitTest", "DrzavaController")]
    [Fact]
    public void VratiIspravanBrojDrzava()
    {
      // Arrange      
      var mockLogger = new Mock<ILogger<DrzavaController>>();

      var dbOptions = new DbContextOptionsBuilder<FirmaContext>()
                .UseInMemoryDatabase(databaseName: "FirmaMemory2")
                .Options;

      using (var context = new FirmaContext(dbOptions))
      {
        for (int i = 0; i < 50; i++)
        {
          context.Add(new Drzava
          {
            OznDrzave = i.ToString()
          });
        }
        context.SaveChanges();
        var controller = new DrzavaController(context, options, mockLogger.Object);
        var tempDataMock = new Mock<ITempDataDictionary>();
        controller.TempData = tempDataMock.Object;

        // Act
        var result = controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        DrzaveViewModel model = Assert.IsType<DrzaveViewModel>(viewResult.Model);
        Assert.Equal(options.Value.PageSize, model.Drzave.Count());        
      }
    }
  }
}
