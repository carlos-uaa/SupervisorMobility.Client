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
using DocumentFormat.OpenXml.Packaging;

namespace SupervisorMobility.Client.Pages.Inicio.HCIPage.Components
{
    public partial class Trajectory
    {
        [Parameter]
        public bool details { get; set; }
        [Parameter]
        public List<UserCareerPath> TrajectoryTable { get; set; }

        [Parameter]
        public EventCallback<UserCareerPath> Add { get; set; }
        [Parameter]
        public EventCallback<int> Del { get; set; }
        [Parameter]
        public EventCallback<(UserCareerPath, int)> Upd { get; set; }

        protected async override Task OnInitializedAsync()
        {
            if (!TrajectoryTable.Any())
            {
                for (int i = 0; i < 5; i++)
                {
                    TrajectoryTable.Add( new UserCareerPath { CareerPathNo = i+1 });
                }
            }
        }

        private void DateChanged(DateTime? range, int index)
        {
            TrajectoryTable.ElementAt(index).ChangeDate = range;
            Upd.InvokeAsync((TrajectoryTable[index], index));
        }
        private void TextChanged(string value, int index, int type)
        {
            switch (type)
            {
                case 0:
                    TrajectoryTable[index].Department = value;
                    break;
                case 1:
                    TrajectoryTable[index].Process = value;
                    break;
                case 2:
                    TrajectoryTable[index].OperationDescription = value;
                    break;
            }
            Upd.InvokeAsync((TrajectoryTable[index], index));
        }
    }
}