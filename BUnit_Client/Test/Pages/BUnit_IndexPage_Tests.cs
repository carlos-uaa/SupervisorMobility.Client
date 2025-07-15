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
using DocumentFormat.OpenXml.Wordprocessing;

namespace BUnit_Client.Test.Pages
{
    public class BUnit_IndexPage_Tests : TestContext
    {

        [Fact]
        public void Index_RendersCorrectly()
        {
            // Arrange
            using var ctx = new IndexPage_Instance();
            // Act
            var cut = ctx.RenderComponent<Index>();

            // Assert
            Assert.NotNull(cut);
        }

        [Fact]
        public void Login_False_RendersCorrectly()
        {
            // Arrange
            using var ctx = new IndexPage_Instance(false);


            // Act
            var cut = ctx.RenderComponent<Index>();

            // Assert
            // 1. El formulario de login existe
            Assert.NotNull(cut.Find("form"));

            // 2. El campo de usuario existe (input de tipo text)
            Assert.NotEmpty(cut.FindAll("input[type='text']"));

            // 3. El campo de contraseña existe (input de tipo password)
            Assert.NotEmpty(cut.FindAll("input[type='password']"));

            // 4. El botón de iniciar sesión existe (puedes buscar por type submit)
            Assert.NotEmpty(cut.FindAll("button[type='submit']"));
        }


        [Fact]
        public void Login_True_RendersCorrectly()
        {
            // Arrange
            using var ctx = new IndexPage_Instance(true);

            // Act
            var cut = ctx.RenderComponent<Index>();

            // Assert

            // Verifica que se renderiza el dashboard (por ejemplo, que existen tarjetas)
            Assert.NotEmpty(cut.FindAll(".menu-card"));

            // Verifica que NO se muestra el formulario de login
            Assert.Empty(cut.FindAll("form"));
        }


        [Fact]
        public void JobObservationSchedule_ViewAll_Button_Navigates()
        {
            using var ctx = new IndexPage_Instance(true);
            var navMan = ctx.Services.GetRequiredService<NavigationManager>() as FakeNavigationManager;
            var cut = ctx.RenderComponent<Index>();

            var btn = cut.FindAll("a")
                .FirstOrDefault(a => a.TextContent.Contains("View all") && a.Closest(".menu-card")?.InnerHtml.Contains(">JOP<") == true);
            Assert.NotNull(btn);
            btn.Click();
            
            Assert.Equal("jobobservationschedule", btn.GetAttribute("href"));

            // Simula la navegación manualmente (opcional, para comprobar el cambio de Uri)
            navMan.NavigateTo(btn.GetAttribute("href"));
            Assert.Contains("jobobservationschedule", navMan.Uri, System.StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void JobObservation_ViewAll_Button_Navigates()
        {
            using var ctx = new IndexPage_Instance(true);
            var navMan = ctx.Services.GetRequiredService<NavigationManager>() as FakeNavigationManager;
            var cut = ctx.RenderComponent<Index>();

            var btn = cut.FindAll("a")
                .FirstOrDefault(a =>
                    a.TextContent.Contains("View all") &&
                    a.Closest(".menu-card")?.InnerHtml.Contains(">JO<") == true);

            Assert.NotNull(btn);
            btn.Click();

            Assert.Equal("jobobservation", btn.GetAttribute("href"));

            // Simula la navegación manualmente (opcional, para comprobar el cambio de Uri)
            navMan.NavigateTo(btn.GetAttribute("href"));
            Assert.Contains("jobobservation", navMan.Uri, System.StringComparison.OrdinalIgnoreCase);
        }


        [Fact]
        public void JobObservation_Create_Button_Navigates()
        {
            using var ctx = new IndexPage_Instance(true);
            var navMan = ctx.Services.GetRequiredService<NavigationManager>() as FakeNavigationManager;
            var cut = ctx.RenderComponent<Index>();

            var btn = cut.FindAll("button")
                .FirstOrDefault(b => b.TextContent.Contains("Create") && b.Closest(".menu-card")?.InnerHtml.Contains(">JO<") == true);
            Assert.NotNull(btn);
            btn.Click();
            Assert.Contains("createnewjobobservation", navMan.Uri, System.StringComparison.OrdinalIgnoreCase);
      

        }

      
        [Fact]
        public void JobObservation_Plan_Button_Navigates()
        {
            using var ctx = new IndexPage_Instance(true);
            var navMan = ctx.Services.GetRequiredService<NavigationManager>() as FakeNavigationManager;
            var cut = ctx.RenderComponent<Index>();

            var btn = cut.FindAll("button")
                .FirstOrDefault(b => b.TextContent.Contains("Plan") && b.Closest(".menu-card")?.InnerHtml.Contains(">JO<") == true);
            Assert.NotNull(btn);
            btn.Click();
            Assert.Contains("planjobobservation", navMan.Uri, System.StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void SosReviewProgram_ViewAll_Button_Navigates()
        {
            using var ctx = new IndexPage_Instance(true);
            var navMan = ctx.Services.GetRequiredService<NavigationManager>() as FakeNavigationManager;
            Assert.NotNull(navMan);
            var cut = ctx.RenderComponent<Index>();

            var btn = cut.FindAll("a")
                .FirstOrDefault(a =>
                    a.TextContent.Contains("View all") &&
                    a.Closest(".menu-card")?.InnerHtml.Contains(">SRP<") == true);

            Assert.NotNull(btn);
            btn.Click();
            Assert.Equal("sosProgram", btn.GetAttribute("href"));
            navMan.NavigateTo(btn.GetAttribute("href"));
            Assert.Contains("sosProgram", navMan.Uri, System.StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void SosReviewProgram_Create_Button_Navigates()
        {
            using var ctx = new IndexPage_Instance(true);
            var navMan = ctx.Services.GetRequiredService<NavigationManager>() as FakeNavigationManager;
            Assert.NotNull(navMan);
            var cut = ctx.RenderComponent<Index>();

            var btn = cut.FindAll("a")
                .FirstOrDefault(a =>
                    a.TextContent.Contains("Create") &&
                    a.Closest(".menu-card")?.InnerHtml.Contains(">SRP<") == true);

            Assert.NotNull(btn);
            btn.Click();
            Assert.Equal("sosProgram/createSosReview", btn.GetAttribute("href"));
            navMan.NavigateTo(btn.GetAttribute("href"));
            Assert.Contains("sosProgram/createSosReview", navMan.Uri, System.StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Lup_ViewAll_Button_Navigates()
        {
            using var ctx = new IndexPage_Instance(true);
            var navMan = ctx.Services.GetRequiredService<NavigationManager>() as FakeNavigationManager;
            Assert.NotNull(navMan);
            var cut = ctx.RenderComponent<Index>();

            var btn = cut.FindAll("a")
                .FirstOrDefault(a =>
                    a.TextContent.Contains("View all") &&
                    a.Closest(".menu-card")?.InnerHtml.Contains(">LUP<") == true);

            Assert.NotNull(btn);
            btn.Click();
            Assert.Equal("lup", btn.GetAttribute("href"));
            navMan.NavigateTo(btn.GetAttribute("href"));
            Assert.Contains("lup", navMan.Uri, System.StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Pat_ViewAll_Button_Navigates()
        {
            using var ctx = new IndexPage_Instance(true);
            var navMan = ctx.Services.GetRequiredService<NavigationManager>() as FakeNavigationManager;
            Assert.NotNull(navMan);
            var cut = ctx.RenderComponent<Index>();

            var btn = cut.FindAll("a")
                .FirstOrDefault(a =>
                    a.TextContent.Contains("View all") &&
                    a.Closest(".menu-card")?.InnerHtml.Contains(">PAT<") == true);

            Assert.NotNull(btn);
            btn.Click();
            Assert.Equal("PAT", btn.GetAttribute("href"));
            navMan.NavigateTo(btn.GetAttribute("href"));
            Assert.Contains("PAT", navMan.Uri, System.StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void HeadCount_ViewAll_Button_Navigates()
        {
            using var ctx = new IndexPage_Instance(true);
            var navMan = ctx.Services.GetRequiredService<NavigationManager>() as FakeNavigationManager;
            Assert.NotNull(navMan);
            var cut = ctx.RenderComponent<Index>();

            var btn = cut.FindAll("a")
                .FirstOrDefault(a =>
                    a.TextContent.Contains("View all") &&
                    a.Closest(".menu-card")?.InnerHtml.Contains(">HC<") == true);

            Assert.NotNull(btn);
            btn.Click();
            Assert.Equal("headCount", btn.GetAttribute("href"));
            navMan.NavigateTo(btn.GetAttribute("href"));
            Assert.Contains("headCount", navMan.Uri, System.StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Kaizen_ViewAll_Button_Navigates()
        {
            using var ctx = new IndexPage_Instance(true);
            var navMan = ctx.Services.GetRequiredService<NavigationManager>() as FakeNavigationManager;
            Assert.NotNull(navMan);
            var cut = ctx.RenderComponent<Index>();

            var btn = cut.FindAll("a")
                .FirstOrDefault(a =>
                    a.TextContent.Contains("View all") &&
                    a.Closest(".menu-card")?.InnerHtml.Contains(">KZN<") == true);

            Assert.NotNull(btn);
            btn.Click();
            Assert.Equal("kaizen", btn.GetAttribute("href"));
            navMan.NavigateTo(btn.GetAttribute("href"));
            Assert.Contains("kaizen", navMan.Uri, System.StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Kaizen_Create_Button_Navigates()
        {
            using var ctx = new IndexPage_Instance(true);
            var navMan = ctx.Services.GetRequiredService<NavigationManager>() as FakeNavigationManager;
            Assert.NotNull(navMan);
            var cut = ctx.RenderComponent<Index>();

            var btn = cut.FindAll("a")
                .FirstOrDefault(a =>
                    a.TextContent.Contains("Create") &&
                    a.Closest(".menu-card")?.InnerHtml.Contains(">KZN<") == true);

            Assert.NotNull(btn);
            btn.Click();
            Assert.Equal("kaizen/createkaizen", btn.GetAttribute("href"));
            navMan.NavigateTo(btn.GetAttribute("href"));
            Assert.Contains("kaizen/createkaizen", navMan.Uri, System.StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Configuration_ViewAll_Button_Navigates()
        {
            using var ctx = new IndexPage_Instance(true);
            var navMan = ctx.Services.GetRequiredService<NavigationManager>() as FakeNavigationManager;
            Assert.NotNull(navMan);
            var cut = ctx.RenderComponent<Index>();

            var btn = cut.FindAll("a")
                .FirstOrDefault(a =>
                    a.TextContent.Contains("View all") &&
                    a.Closest(".menu-card")?.InnerHtml.Contains(">CONFIG<") == true);

            Assert.NotNull(btn);
            btn.Click();
            Assert.Equal("configuration", btn.GetAttribute("href"));
            navMan.NavigateTo(btn.GetAttribute("href"));
            Assert.Contains("configuration", navMan.Uri, System.StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void SOSHOE_ViewAll_Button_Navigates()
        {
            using var ctx = new IndexPage_Instance(true);
            var navMan = ctx.Services.GetRequiredService<NavigationManager>() as FakeNavigationManager;
            Assert.NotNull(navMan);
            var cut = ctx.RenderComponent<Index>();

            var btn = cut.FindAll("a")
                .FirstOrDefault(a =>
                    a.TextContent.Contains("View all") &&
                    a.Closest(".menu-card")?.InnerHtml.Contains(">SOS<") == true);

            Assert.NotNull(btn);
            btn.Click();
            Assert.Equal("SOSHOE", btn.GetAttribute("href"));
            navMan.NavigateTo(btn.GetAttribute("href"));
            Assert.Contains("SOSHOE", navMan.Uri, System.StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void IS_ViewAll_Button_Navigates()
        {
            using var ctx = new IndexPage_Instance(true);
            var navMan = ctx.Services.GetRequiredService<NavigationManager>() as FakeNavigationManager;
            Assert.NotNull(navMan);
            var cut = ctx.RenderComponent<Index>();

            var btn = cut.FindAll("a")
                .FirstOrDefault(a =>
                    a.TextContent.Contains("View all") &&
                    a.Closest(".menu-card")?.InnerHtml.Contains(">IS<") == true);

            Assert.NotNull(btn);
            btn.Click();
            Assert.Equal("IS", btn.GetAttribute("href"));
            navMan.NavigateTo(btn.GetAttribute("href"));
            Assert.Contains("IS", navMan.Uri, System.StringComparison.OrdinalIgnoreCase);
        }

    }
}



