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
    public partial class Trajectory
    {
        [Parameter]
        public Dictionary<int, dicValuesTraject> TrajectoryTable { get; set; } = new Dictionary<int, dicValuesTraject>();

        protected async override Task OnInitializedAsync()
        {
            if (!TrajectoryTable.Any())
            {
                for (int i = 0; i < 5; i++)
                {
                    TrajectoryTable.Add(i, new());
                }
            }
        }
    }

    public class dicValuesTraject
    {
        public int? No;
        public DateTime? dateOchange;
        public string dpt;
        public string processName;
        public string operationDesc;
    }
}