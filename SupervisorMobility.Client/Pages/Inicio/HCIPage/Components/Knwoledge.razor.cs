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
    public partial class Knwoledge
    {
        [Parameter]
        public bool details { get; set; }
        [Parameter]
        public string Title { get; set; } = "notitle";
        [Parameter]
        public int type { get; set; } = 1;

        [Parameter]
        public List<HCITransaction> KnowledgeTable { get; set; }

        [Parameter]
        public EventCallback<HCITransaction> Add { get; set; }
        [Parameter]
        public EventCallback<int> Del { get; set; }
        [Parameter]
        public EventCallback<(HCITransaction, int)> Upd { get; set; }


        protected async override Task OnInitializedAsync()
        {
            if (!KnowledgeTable.Any())
            {
                for(int i = 0; i < 5; i++)
                {
                    KnowledgeTable.Add(new HCITransaction { Type=type });
                }
            }
        }

        private void DateChanged(DateRange range, int index)
        {
            KnowledgeTable.ElementAt(index).DateStart = range.Start;
            KnowledgeTable.ElementAt(index).DateEnd = range.End;
            Upd.InvokeAsync((KnowledgeTable[index], index));
        }
        private void TextChanged(string val, int idx)
        {
            KnowledgeTable[idx].Description = val;
            Upd.InvokeAsync((KnowledgeTable[idx], idx));
        }
    }
}