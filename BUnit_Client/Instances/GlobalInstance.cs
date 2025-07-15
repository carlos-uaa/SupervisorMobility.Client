using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Moq;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Resources;
using SupervisorMobility.Client.Services.BreadcrumsService;
using SupervisorMobility.Client.Services.GlobalDataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUnit_Client.Instances
{
    public class GlobalInstance : TestContext
    {
        public GlobalInstance(bool isLogged = true, int userType = 1)
        {
            // Mock usuario
            var user = new User { UserType = userType };
            var globalData = new Mock<GlobalDataService>();
            globalData.SetupProperty(g => g.LoggedUser, "TestUser");

            Services.AddSingleton(Mock.Of<IBreadcrumbService>());
            Services.AddSingleton(Mock.Of<IStringLocalizer<Resource>>());
            Services.AddSingleton(Mock.Of<IStringLocalizer<Resource1>>());
            Services.AddSingleton(Mock.Of<ISnackbar>());
            Services.AddSingleton(globalData.Object);
            Services.AddSingleton(new NavigationManagerForTests());
            Services.AddSingleton(Mock.Of<IJSRuntime>());

            // Simula el estado de login y usuario
            Services.AddSingleton(user);
        }

        // Mock NavigationManager para pruebas
        public class NavigationManagerForTests : NavigationManager
        {
            public NavigationManagerForTests() => Initialize("http://localhost/", "http://localhost/");
            protected override void NavigateToCore(string uri, bool forceLoad) { }
        }
    }
}
