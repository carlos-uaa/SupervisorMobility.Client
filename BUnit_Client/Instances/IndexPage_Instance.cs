using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Resources;
using SupervisorMobility.Client.Pages;
using SupervisorMobility.Client.Services.GlobalDataService;
using SupervisorMobility.Client.Services.LoginService;
using SupervisorMobility.Client.Services.UserService;
using System.Linq;
using Moq;
using Xunit;
using SupervisorMobility.Client.Services.BreadcrumsService;
using Bunit.JSInterop;
using Blazor.Diagrams.Components.Renderers; // Add this using directive at the top of the file

namespace BUnit_Client
{
    public class IndexPage_Instance : TestContext
    {
        public string userAdminTest = @"{""UserId"":2,""Payroll"":null,""ObjectId"":""maguayosinco@compasdcpcs.local"",""Name"":""Marco Aguayo"",""Email"":""maguayo@gruposinco.com.mx"",""UserType"":1,""SuperiorId"":null,""Superior"":null,""Subordinates"":[],""ILURegisers"":[],""CreatedDate"":""2023-10-05T00:00:00"",""LastUpdated"":""0001-01-01T00:00:00"",""DisabledDate"":null,""IsActive"":true,""PlantId"":null,""Plant"":null,""AreaId"":null,""Area"":null,""Areas"":[],""GroupId"":null,""Group"":null,""DistributionId"":null,""Distribution"":null}";

        public IndexPage_Instance()
        {
            // Mock de servicios requeridos
            Services.AddSingleton(Mock.Of<ILoginService>());
            Services.AddSingleton(Mock.Of<IBreadcrumbService>());
            Services.AddSingleton(Mock.Of<IDialogService>());
            Services.AddSingleton(Mock.Of<IUserService>());
            Services.AddSingleton(Mock.Of<ISnackbar>());
            Services.AddSingleton(Mock.Of<GlobalDataService>());
            Services.AddSingleton(Mock.Of<IStringLocalizer<Resource>>());
            Services.AddSingleton(Mock.Of<IStringLocalizer<Resource1>>());
            Services.AddSingleton(Mock.Of<IStringLocalizer<Resource2>>()); 
            Services.AddSingleton(Mock.Of<NavigationManager>(MockBehavior.Loose));
            Services.AddSingleton(Mock.Of<Microsoft.JSInterop.IJSRuntime>());
            Services.AddSingleton(Mock.Of<Microsoft.AspNetCore.Components.WebAssembly.Hosting.IWebAssemblyHostEnvironment>());
        }

        public IndexPage_Instance(bool userLogged = false)
        {
            // Mock de servicios requeridos
            Services.AddSingleton(Mock.Of<ILoginService>());
            Services.AddSingleton(Mock.Of<IBreadcrumbService>());
            Services.AddSingleton(Mock.Of<IDialogService>());
            Services.AddSingleton(Mock.Of<IUserService>());
            Services.AddSingleton(Mock.Of<ISnackbar>());
            Services.AddSingleton(Mock.Of<GlobalDataService>());

            // Configura el mock del localizador para devolver la clave como valor
            var localizerMock = new Mock<IStringLocalizer<Resource>>();
            localizerMock.Setup(l => l[It.IsAny<string>()])
                .Returns((string key) => new LocalizedString(key, key));
            Services.AddSingleton(typeof(IStringLocalizer<Resource>), localizerMock.Object);

            var localizer1Mock = new Mock<IStringLocalizer<Resource1>>();
            localizer1Mock.Setup(l => l[It.IsAny<string>()])
                .Returns((string key) => new LocalizedString(key, key));
            Services.AddSingleton(typeof(IStringLocalizer<Resource1>), localizer1Mock.Object);

            var localizer2Mock = new Mock<IStringLocalizer<Resource2>>();
            localizer2Mock.Setup(l => l[It.IsAny<string>()])
                .Returns((string key) => new LocalizedString(key, key));
            Services.AddSingleton(typeof(IStringLocalizer<Resource2>), localizer2Mock.Object);

            Services.AddSingleton<NavigationManager, FakeNavigationManager>();

            Services.AddSingleton<MudBlazor.IJsApiService>(Mock.Of<MudBlazor.IJsApiService>());
            Services.AddSingleton(Mock.Of<Microsoft.AspNetCore.Components.WebAssembly.Hosting.IWebAssemblyHostEnvironment>());

            // Mock manual de IJSRuntime
            var jsMock = new Mock<Microsoft.JSInterop.IJSRuntime>();

            // Configura el mock para localStorage.hasOwnProperty
            jsMock.Setup(js => js.InvokeAsync<bool>(
                    "localStorage.hasOwnProperty",
                    It.IsAny<object[]>()
                )).ReturnsAsync(userLogged);

            // Configura el mock para localStorage.getItem
            jsMock.Setup(js => js.InvokeAsync<string>(
                    "localStorage.getItem",
                    It.IsAny<object[]>()
                )).ReturnsAsync(userLogged ? userAdminTest : null);

            Services.AddSingleton(jsMock.Object);


        }

    }
}
