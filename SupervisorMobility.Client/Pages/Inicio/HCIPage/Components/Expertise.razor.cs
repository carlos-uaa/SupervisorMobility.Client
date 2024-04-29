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
    public partial class Expertise
    {
        [Parameter]
        public bool details { get; set; }
        [Parameter]
        public List<HCIILU> ExpertiseTable { get; set; }


        [Parameter]
        public EventCallback<HCIILU> Add { get; set; }
        [Parameter]
        public EventCallback<int> Del { get; set; }
        [Parameter]
        public EventCallback<(HCIILU, int)> Upd { get; set; }

        protected async override Task OnInitializedAsync()
        {
            if (!ExpertiseTable.Any())
            {
                for (int i = 0; i < 5; i++)
                {
                    ExpertiseTable.Add( new());
                }
            }
        }

        private void DateChanged(DateRange range, int index)
        {
            ExpertiseTable[index].Start = range.Start;
            ExpertiseTable[index].End = range.End;
            Upd.InvokeAsync((ExpertiseTable[index], index));
        }
        private void TextChanged(string val, int idx)
        {
            ExpertiseTable[idx].Description = val;
            Upd.InvokeAsync((ExpertiseTable[idx], idx));
        }
        private void LevelChanged(string val, int idx)
        {
            ExpertiseTable[idx].level = val;
            Upd.InvokeAsync((ExpertiseTable[idx], idx));
        }

        private void Delete(int index)
        {
            //KnowledgeTable.RemoveAt(index);
            Del.InvokeAsync(index);
        }

        private void AddHere()
        {
            HCIILU niu = new HCIILU();
            //KnowledgeTable.Add(niu);
            Add.InvokeAsync(niu);
        }
    }
}