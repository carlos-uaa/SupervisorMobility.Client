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
using Blazor.Diagrams.Components.Renderers;

namespace BUnit_Client.Test
{
    /// <summary>
    /// These tests are written entirely in C#.
    /// Learn more at https://bunit.dev/docs/getting-started/writing-tests.html#creating-basic-tests-in-cs-files
    /// </summary>
    public class PruebaYUsos : TestContext
    {
        public PruebaYUsos()
        {
            // Mock de servicios requeridos
            Services.AddSingleton(Mock.Of<ILoginService>());
            Services.AddSingleton(Mock.Of<IBreadcrumbService>());
            Services.AddSingleton(Mock.Of<IDialogService>());
            Services.AddSingleton(Mock.Of<IUserService>());
            Services.AddSingleton(Mock.Of<ISnackbar>());
            Services.AddSingleton(Mock.Of<GlobalDataService>());
            Services.AddSingleton(typeof(IStringLocalizer<Resource>), Mock.Of<IStringLocalizer<Resource>>());


            Services.AddSingleton(Mock.Of<NavigationManager>(MockBehavior.Loose));
            Services.AddSingleton(Mock.Of<Microsoft.JSInterop.IJSRuntime>());
            Services.AddSingleton(Mock.Of<Microsoft.AspNetCore.Components.WebAssembly.Hosting.IWebAssemblyHostEnvironment>());

            // Simula un usuario logueado
            Services.AddSingleton(new User { UserType = 1 }); // Ajusta seg�n tu modelo real
        }

        [Fact]
        public void HelloWorldComponentRendersCorrectly()
        {
            // Act
            var cut = RenderComponent<Index>();
            // Assert
            cut.MarkupMatches("<h1>Hello world from Blazor</h1>");
            //Esta funcion compara el html que le pasamos y espera que sean idénticos
            //(sin elementos extra, ni estilos, ni otros componentes).

            //Componentes pequeños, quiza un boton
        }

        [Fact]
        public void FindHelloWorldComponentRendersCorrectly()
        {
            // Act
            var cut = RenderComponent<Index>();

            // Assert
            // Busca el titulo (por ejemplo, en un <h1>)
            var titulo = cut.Find("h1");
            Assert.Equal("Hello world from Blazor", titulo.TextContent);

            //Aquí solo buscas el elemento <h1> y comparas su texto, sin importar qué otros elementos existan en el HTML.
            //Esto es mucho más flexible y robusto, porque solo te importa que el<h1> tenga el texto correcto, no el resto del HTML
        }

        [Fact]
        public void FindMultipleH1()
        {
            // Act
            var cut = RenderComponent<Index>();

            // Assert
            // Busca todos titulo (por ejemplo, en un <h1>)

            var titulos = cut.FindAll("h1");
            Assert.Equal("Hello world from Blazor", titulos[0].TextContent);
            Assert.Equal("Godbye world from Blazor", titulos[1].TextContent);

            //Puede ser util para cuando se renderizan varios encabezados, secciones o 
            // en una pagina, como en un dashboard o una lista de elementos,
            // y quieres verificar que todos tengan el texto correcto.
        }

        [Fact]
        public void MarkUpHelloWorld_RendersCorrectly_UsingTestContext()
        {
            // Arrange
            using var ctx = new TestContext();

            // Act
            var cut = ctx.RenderComponent<Index>();

            // Assert
            cut.MarkupMatches("<h1>Hello world from Blazor</h1>");
        }

        [Fact]
        public void MarkUpHelloWorld_RendersCorrectly_Instantiating()
        {
            // Arrange
            using var ctx = new PruebaYUsos();

            // Act
            var cut = ctx.RenderComponent<Index>();

            // Assert
            cut.MarkupMatches("<h1>Hello world from Blazor</h1>");
        }

        [Fact]
        public void FindHelloWorld_RendersCorrectly_instantiating()
        {
            // Arrange
            using var ctx = new PruebaYUsos();

            // Act
            var cut = ctx.RenderComponent<Index>();

            // Assert
            var titulos = cut.FindAll("h1");
            Assert.Equal("Hello world from Blazor", titulos[0].TextContent);
            Assert.Equal("Godbye world from Blazor", titulos[1].TextContent);
        }


        [Fact]
        public void Should_Render_SRP_Section_Buttons_And_Texts()
        {
            // Mock de JSInterop para simular usuario logueado en localStorage
            var jsMock = JSInterop.SetupModule();

            jsMock.Setup<bool>("localStorage.hasOwnProperty", It.IsAny<object[]>()).SetResult(true);
            var user = new User { Name = "Test User", UserType = 1 };
            var userJson = System.Text.Json.JsonSerializer.Serialize(user);
            jsMock.Setup<string>("localStorage.getItem", It.IsAny<object[]>()).SetResult(userJson);

            // Mock de IStringLocalizer para los textos usados en la secci�n SRP
            var localizerMock = new Mock<IStringLocalizer<Resource>>();
            localizerMock.Setup(l => l["sosProgram"]).Returns(new LocalizedString("sosProgram", "SOS Review Program"));
            localizerMock.Setup(l => l["sosProgramDescription"]).Returns(new LocalizedString("sosProgramDescription", "Descripci�n del programa SRP"));
          
            Services.AddSingleton(typeof(IStringLocalizer<Resource>), localizerMock.Object);


            // Renderiza el componente
            //using var ctx = new Bunit_Buttons_IndexPage_Test();
            var cut = RenderComponent<Index>(); // Cambiar SupervisorMobility.Client.Pages.Index por Index

            // Busca el t�tulo (por ejemplo, en un <h6>)
            var titulo = cut.Find("h6");
            Assert.Equal("SOS Review Program", titulo.TextContent);

            // Busca la descripci�n (por ejemplo, en un <p>)
            var descripcion = cut.Find("p");
            Assert.Equal("Descripci�n del programa SRP", descripcion.TextContent);
        }

        [Fact]
        public void Should_Render_SRP_Menu_Card_Correctly()
        {
            // Arrange: Mock the localizer for the expected keys
            var localizerMock = new Mock<IStringLocalizer<Resource>>();
            localizerMock.Setup(l => l["sosProgram"]).Returns(new LocalizedString("sosProgram", "SOS Review Program"));
            localizerMock.Setup(l => l["sosProgramDescription"]).Returns(new LocalizedString("sosProgramDescription", "Description of the SRP program"));
            Services.AddSingleton(typeof(IStringLocalizer<Resource>), localizerMock.Object);

            // Act: Render the Index page
            var cut = RenderComponent<Index>();

            // Assert: Find the MudChip with "SRP"
            var chip = cut.Find("span.mud-chip-content");
            Assert.Equal("SRP", chip.TextContent);

            // Assert: Find the MudText with the program name
            var title = cut.FindAll("div.header .mud-typography").FirstOrDefault();
            Assert.NotNull(title);
            Assert.Equal("SOS Review Program", title.TextContent);

            // Assert: Find the MudImage with the correct src
            var img = cut.Find("img");
            Assert.Equal("Images/documento.png", img.GetAttribute("src"));

            // Assert: Find the MudText with the program description
            var description = cut.FindAll("div.content .mud-typography").FirstOrDefault();
            Assert.NotNull(description);
            Assert.Equal("Description of the SRP program", description.TextContent);

            // Assert: Find the two MudButtons
            var buttons = cut.FindAll("button");
            Assert.Contains(buttons, b => b.TextContent.Contains("View all"));
            Assert.Contains(buttons, b => b.TextContent.Contains("Create"));
        }


        [Fact]
        public void Should_Invoke_CreateNewJobObservation_OnClick()
        {
            // Arrange
            var cut = RenderComponent<Index>();

            // Busca el bot�n "Create" de Job Observation
            var createButton = cut.FindAll("button").FirstOrDefault(b => b.TextContent.Contains("Create"));

            // Act & Assert
            Assert.NotNull(createButton);
            createButton.Click();
            // Aqu� podr�as verificar efectos secundarios si el m�todo modifica el estado o navega
        }

        [Fact]
        public void Should_Invoke_PlanJobObservation_OnClick()
        {
            // Arrange
            var cut = RenderComponent<Index>();

            // Busca el bot�n "Plan" de Job Observation
            var planButton = cut.FindAll("button").FirstOrDefault(b => b.TextContent.Contains("Plan"));

            // Act & Assert
            Assert.NotNull(planButton);
            planButton.Click();
            // Aqu� podr�as verificar efectos secundarios si el m�todo modifica el estado o navega
        }

    }
}

