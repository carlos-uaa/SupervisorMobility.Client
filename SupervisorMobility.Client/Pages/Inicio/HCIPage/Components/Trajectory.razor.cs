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
using SupervisorMobility.Client.Data.Entities;
using System.Globalization;

namespace SupervisorMobility.Client.Pages.Inicio.HCIPage.Components
{
    public partial class Trajectory
    {
        [Parameter]
        public bool details { get; set; }
        [Parameter]
        public List<UserCareerPath> TrajectoryTable { get; set; }
    }
}