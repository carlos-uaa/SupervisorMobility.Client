using AngleSharp.Dom;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Moq;
using MudBlazor;
using MudBlazor.Services;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Resources;
using SupervisorMobility.Client.Services.AreaService;
using SupervisorMobility.Client.Services.BreadcrumsService;
using SupervisorMobility.Client.Services.GlobalDataService;
using SupervisorMobility.Client.Services.LoginService;
using SupervisorMobility.Client.Services.PlantService;
using SupervisorMobility.Client.Services.UserService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BUnit_Client.Pages.Configuration.Plant
{
    public class PlantPageDetails_Instance : TestContext
    {
        public PlantPageDetails_Instance()
        {
            // Mock de IBreadcrumbService
            Services.AddSingleton(Mock.Of<IBreadcrumbService>());

            // HttpClient scoped
            Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:10201/api/"), Timeout = TimeSpan.FromMinutes(15) });

            // Servicios de dominio como scoped
            //Services.AddScoped<IPlantService, PlantService>();
            //Services.AddScoped<IAreaService, AreaService>();

            // Mock de IPlantService con funciones de test
            var PlantServiceMock = new Mock<IPlantService>();
            // Ejemplo: Setup para GetPlantByIdAsync
            PlantServiceMock.Setup(s => s.GetPlantById(It.IsAny<int>()))
                .ReturnsAsync((int id) => new Entity. .Plant
                {
                    PlantId = id,
                    Code = "PLANT" + id,
                    Description = "Plant Description " + id,
                    IsActive = true
                });
            Services.AddScoped(_ => PlantServiceMock.Object);

            // NavigationManager fake
            Services.AddSingleton<NavigationManager, FakeNavigationManager>();

            // MudBlazor: registra todos los servicios necesarios para pruebas
            Services.AddMudServices();

            // Otros mocks
            Services.AddSingleton(Mock.Of<IJSRuntime>());
            Services.AddSingleton(Mock.Of<IStringLocalizer<Resource>>());
            Services.AddSingleton(Mock.Of<IStringLocalizer<Resource1>>());
            Services.AddSingleton(Mock.Of<IStringLocalizer<Resource2>>());
            Services.AddSingleton(Mock.Of<IDialogService>());
            Services.AddSingleton(Mock.Of<ILoginService>());
            Services.AddSingleton(Mock.Of<IUserService>());
            Services.AddSingleton(Mock.Of<ISnackbar>());
            Services.AddSingleton(Mock.Of<GlobalDataService>());
            Services.AddSingleton(Mock.Of<Microsoft.AspNetCore.Components.WebAssembly.Hosting.IWebAssemblyHostEnvironment>());
        }
    }
}
