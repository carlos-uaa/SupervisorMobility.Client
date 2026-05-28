using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using SupervisorMobility.Client;
using SupervisorMobility.Client.Shared;
using SupervisorMobility.Client.Services;
using SupervisorMobility.Client.Data.Resources;
using Microsoft.Extensions.Localization;
using MudBlazor;
using BlazorCameraStreamer;
using Blazored.SessionStorage;

namespace SupervisorMobility.Client.Pages.Inicio.HCIPage.Components
{
    public partial class Category
    {
        [Parameter]
        public bool details { get; set; }

        [Parameter]
        public List<HCICategory> CategoryTable { get; set; }

        [Parameter]
        public EventCallback<HCICategory> Add { get; set; }
        [Parameter]
        public EventCallback<int> Del { get; set; }
        [Parameter]
        public EventCallback<(HCICategory, int)> Upd { get; set; }

        public List<Department> ExistingCategories = new List<Department>();
        public bool dataloaded;

        protected async override Task OnInitializedAsync()
        {
            ExistingCategories = await DepartmentService.GetDepartments() ?? new();
            if (!CategoryTable.Any())
                    CategoryTable.Add( new());
            dataloaded = true;
        }

        private void DateChanged(DateTime? range, int index)
        {
            CategoryTable.ElementAt(index).Date = range;
            Upd.InvokeAsync((CategoryTable[index], index));
        }
        private void OptionChanged(int opt, int idx) //Changethis for the select
        {
            CategoryTable[idx].ChosenCategoryDepartmentId = opt;
            Upd.InvokeAsync((CategoryTable[idx], idx));
        }

        private void Delete(int index)
        {
            //KnowledgeTable.RemoveAt(index);
            Del.InvokeAsync(index);
        }

        private void AddHere()
        {
            HCICategory niu = new HCICategory();
            //KnowledgeTable.Add(niu);
            Add.InvokeAsync(niu);
        }
    }
}