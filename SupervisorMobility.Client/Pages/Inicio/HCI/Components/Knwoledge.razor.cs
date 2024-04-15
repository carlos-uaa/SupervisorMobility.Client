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

namespace SupervisorMobility.Client.Pages.Inicio.HCI.Components
{
    public partial class Knwoledge
    {
        [Parameter]
        public string Title { get; set; } = "notitle";

        [Parameter]
        public Dictionary<int, dicValues> KnowledgeTable { get; set; } = new Dictionary<int, dicValues>();

        protected async override Task OnInitializedAsync()
        {
            if (!KnowledgeTable.Any())
            {
                for(int i = 0; i < 5; i++)
                {
                    KnowledgeTable.Add(i, new());
                }
            }
        }
    }

    public class dicValues
    {
        public DateRange period;
        public string topic;
    }
}