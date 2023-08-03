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
using DocumentFormat.OpenXml.Spreadsheet;

namespace SupervisorMobility.Client.Pages.Configuration.GlosaryPage
{
    public partial class GlosaryPage
    {
        [CascadingParameter]
        public Dictionary<string, Glosary> _glosaryInfo { get; set; }

        private List<BreadcrumbItem> _links;
        // Objects
        public List<Glosary> _glosary { get; set; } = new();

        // Initialization
        protected async override Task OnInitializedAsync()
        {
            _links = new List<BreadcrumbItem>{
            new BreadcrumbItem(text: Localizer["home"], href: "/", disabled:false),
            new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration", disabled:false),
            new BreadcrumbItem(text: Localizer["GlosaryTitle"], href: "", disabled: true)
            };

            _glosary = await GlosaryService.GetGlosary();
        }

        void CreateGlosaryWord()
        {
            NavigationManager.NavigateTo($"glosary/createglosaryWord");
        }

        async Task DeleteGlosaryWord(int glosaryWordId)
        {
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"{Localizer["GlosaryDeleteMsg"]}");

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

        private int selectedRowNumber = -1;
        private MudTable<Glosary> SelectTableEvent;

        private void RowClickEvent(TableRowClickEventArgs<Glosary> tableRowClickEventArgs)
        {
        }

       
        private string SelectedRowClassFunc(Glosary element, int rowNumber)
        {
            if (selectedRowNumber == rowNumber)
            {
                selectedRowNumber = -1;
                if (SelectTableEvent.SelectedItem != null && SelectTableEvent.SelectedItem.Equals(element))
                {
                     NavigationManager.NavigateTo($"glosary/updateglosaryword/{element.GlosaryWordId}");

                }
                return string.Empty;
            }
            else if (SelectTableEvent.SelectedItem != null && SelectTableEvent.SelectedItem.Equals(element))
            {
                selectedRowNumber = rowNumber;
                return "selected";
            }
            else
            {
                return string.Empty;
            }
        }
    }
}