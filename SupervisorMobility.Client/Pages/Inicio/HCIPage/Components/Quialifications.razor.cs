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
    public partial class Quialifications
    {
        [Parameter]
        public bool details { get; set; }
        [Parameter]
        public List<HCITransaction> QualificationsTable { get; set; }

        [Parameter]
        public EventCallback<HCITransaction> Add { get; set; }
        [Parameter]
        public EventCallback<int> Del { get; set; }
        [Parameter]
        public EventCallback<(HCITransaction, int)> Upd { get; set; }


        protected async override Task OnInitializedAsync()
        {
            if (!QualificationsTable.Any())
            {
                QualificationsTable.Add(new HCITransaction { Type = 3, IsActive = true });
            }
        }

        private void DateChanged(DateTime? range, int index)
        {
            QualificationsTable.ElementAt(index).DateEnd = range;
            Upd.InvokeAsync((QualificationsTable[index], index));
        }
        private void TextChanged(string val, int idx)
        {
            QualificationsTable[idx].Description = val;
            Upd.InvokeAsync((QualificationsTable[idx], idx));
        }

        private void Delete(int index)
        {
            //KnowledgeTable.RemoveAt(index);
            Del.InvokeAsync(index);
        }

        private void AddHere()
        {
            HCITransaction niu = new HCITransaction { Type = 3, IsActive = true };
            //KnowledgeTable.Add(niu);
            Add.InvokeAsync(niu);
        }
    }

}