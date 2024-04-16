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

namespace SupervisorMobility.Client.Pages.Inicio.HCIPage
{
    public partial class HCIForm
    {
        [Parameter]
        public int? HCIID { get; set; }

        [SupplyParameterFromQuery]
        public int? userId { get; set; }

        public Data.Entities.HCI _hci = new ();
        public bool Details = false;

        public List<HCITransaction> manualCap = new();
        public List<HCITransaction> companyCap = new();
        public List<HCITransaction> titles = new();
        public List<UserCareerPath> careers = new();
        public List<ILURegister> expertise = new();
        public List<Commentary> comments = new();

        protected async override Task OnInitializedAsync()
        {
            var currentUrl = NavManager.Uri;
            Details = currentUrl.Contains("Details", StringComparison.OrdinalIgnoreCase);

            if (HCIID == null)
            {
                if(userId != null)
                {
                    _hci.User = await UsrsService.GetUser(userId.Value);
                    
                }
                else
                {
                    //redirect or sumthin'
                }
            }
            else 
            {
                _hci = await HCIService.GetHCI(HCIID.Value);
                manualCap = _hci.Transactions?.Where(p=>p.Type == 1).ToList() ?? new();
                companyCap = _hci.Transactions?.Where(p => p.Type == 2).ToList() ?? new();
                titles = _hci.Transactions?.Where(p => p.Type == 3).ToList() ?? new();
                careers = _hci.User?.UserCareerPaths?.ToList() ?? new();
                expertise = _hci.User?.ILURegisers?.ToList() ?? new();
                comments = _hci.Comments?.ToList() ?? new();
            }
        }
    }
}