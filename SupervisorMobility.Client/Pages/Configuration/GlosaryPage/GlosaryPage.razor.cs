using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client;
using SupervisorMobility.Client.Shared;
using SupervisorMobility.Client.Services;

namespace SupervisorMobility.Client.Pages.Configuration.GlosaryPage
{
    public partial class GlosaryPage
    {
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>{
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Glosary", href: "", disabled: true)
        };

        // Objects
        public List<Glosary> _glosary { get; set; } = new();

        // Initialization
        protected async override Task OnInitializedAsync()
        {
            _glosary = await GlosaryService.GetGlosary();
        }

        void CreateGlosaryWord()
        {
            NavigationManager.NavigateTo($"glosary/createglosaryWord");
        }

        async Task DeleteGlosaryWord(int glosaryWordId)
        {
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this Word?");

            if (confirm)
            {
                _glosary.RemoveAll(glosaryWord => glosaryWord.GlosaryWordId == glosaryWordId);
                await GlosaryService.DeleteGlosaryWord(glosaryWordId);
            }
        }
        
        void UpdateGlosaryWord(int glosaryWordId)
        {
            NavigationManager.NavigateTo($"glosary/updateglosaryword/{glosaryWordId}");
        }

        private string searchString = "";

        private bool FilterFunc(Glosary element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.GlosaryWordId.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{element.GlosaryWordId} {element.Name} {element.Description}".Contains(searchString))
                return true;
            return false;
        }
    }
}