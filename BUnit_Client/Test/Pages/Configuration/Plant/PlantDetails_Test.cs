using BUnit_Client.Pages.Configuration.Plant;
using Moq;
using MudBlazor.Services;
using SupervisorMobility.Client.Pages.Configuration.PlantPage;
using SupervisorMobility.Client.Services.PlantService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUnit_Client.Test.Pages.Configuration.Plant
{
    public class PlantDetails_Test : TestContext
    {
        [Fact]
        public void PlantsRendersCorrectly()
        {
            // Arrange
            using var ctx = new PlantPageDetails_Instance();
            ctx.JSInterop.Mode = JSRuntimeMode.Loose; // Permite llamadas JS ficticias

            // Act
            var cut = ctx.RenderComponent<PlantDetail>(parameters => parameters.Add(p => p.PlantId, 1));
            // Assert
            Assert.NotNull(cut);
        }

        [Fact]
        public void PlantDetail_Card_Exists()
        {
            using var ctx = new PlantPageDetails_Instance();
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;

            var cut = ctx.RenderComponent<PlantDetail>(parameters => parameters.Add(p => p.PlantId, 1));

            // Busca la tarjeta principal (mud-card)
            var card = cut.FindAll(".mud-card");
            Assert.NotEmpty(card);

            // Busca el header de la tarjeta
            var header = cut.FindAll(".mud-card-header");
            Assert.NotEmpty(header);

            // Busca el contenido de la tarjeta
            var content = cut.FindAll(".mud-card-content");
            Assert.NotEmpty(content);
        }

        [Fact]
        public void PlantDetail_Tabs_And_Buttons_Exist()
        {
            using var ctx = new PlantPageDetails_Instance();
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;

            var cut = ctx.RenderComponent<PlantDetail>(parameters => parameters.Add(p => p.PlantId, 1));

            // Busca los tabs
            var tabs = cut.FindAll(".mud-tabs");
            Assert.NotEmpty(tabs);

            // Busca los botones dentro de la tarjeta
            var buttons = cut.FindAll("button");
            Assert.NotEmpty(buttons);
        }


        [Fact]
        public void PlantDetail_Should_Render_Main_Card()
        {
            using var ctx = new PlantPageDetails_Instance();
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;

            var cut = ctx.RenderComponent<PlantDetail>(parameters => parameters.Add(p => p.PlantId, 1));

            // Verifica que existe al menos una tarjeta mud-card
            Assert.NotEmpty(cut.FindAll(".mud-card"));
        }

        [Fact]
        public void PlantDetail_Should_Render_Card_Header()
        {
            using var ctx = new PlantPageDetails_Instance();
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;

            var cut = ctx.RenderComponent<PlantDetail>(parameters => parameters.Add(p => p.PlantId, 1));

            // Verifica que existe el header de la tarjeta
            Assert.NotEmpty(cut.FindAll(".mud-card-header"));
        }

        [Fact]
        public void PlantDetail_Should_Render_Card_Content()
        {
            using var ctx = new PlantPageDetails_Instance();
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;

            var cut = ctx.RenderComponent<PlantDetail>(parameters => parameters.Add(p => p.PlantId, 1));

            // Verifica que existe el contenido de la tarjeta
            Assert.NotEmpty(cut.FindAll(".mud-card-content"));
        }

        [Fact]
        public void PlantDetail_Should_Render_Chip()
        {
            using var ctx = new PlantPageDetails_Instance();
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;

            var cut = ctx.RenderComponent<PlantDetail>(parameters => parameters.Add(p => p.PlantId, 1));

            // Verifica que existe al menos un chip
            Assert.NotEmpty(cut.FindAll(".mud-chip"));
        }


        [Fact]
        public void PlantDetail_Should_Render_Tabs()
        {
            using var ctx = new PlantPageDetails_Instance();
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;

            var cut = ctx.RenderComponent<PlantDetail>(parameters => parameters.Add(p => p.PlantId, 1));

            // Verifica que existen tabs
            Assert.NotEmpty(cut.FindAll(".mud-tabs"));
        }

        [Fact]
        public void PlantDetail_Should_Render_Buttons()
        {
            using var ctx = new PlantPageDetails_Instance();
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;

            var cut = ctx.RenderComponent<PlantDetail>(parameters => parameters.Add(p => p.PlantId, 1));

            // Verifica que existe al menos un botón
            Assert.NotEmpty(cut.FindAll("button"));
        }


    }
}
