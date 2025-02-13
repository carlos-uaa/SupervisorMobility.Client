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
        public List<ILURegister> ExpertiseTable { get; set; }


        [Parameter]
        public EventCallback<ILURegister> Add { get; set; }
        [Parameter]
        public EventCallback<int> Del { get; set; }
        [Parameter]
        public EventCallback<(ILURegister, int)> Upd { get; set; }

        private List<ILULevel> _LevelsILU { get; set; } = new();
        private ILURegister _newIlu { get; set; } = new();


        protected async override Task OnInitializedAsync()
        {
            if (!ExpertiseTable.Any())
            {
                for (int i = 0; i < 5; i++)
                {
                    ExpertiseTable.Add( new());
                }
            }
            _LevelsILU = await ILUServices.GetLevelsILU();

            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private void DateChanged(DateRange range, int index)
        {
            ExpertiseTable[index].AcquisitionDate = range.Start;
            ExpertiseTable[index].EndDate = range.End;
            Upd.InvokeAsync((ExpertiseTable[index], index));
        }
        //private void TextChanged(string val, int idx)
        //{
        //    ExpertiseTable[idx].Description = val;
        //    Upd.InvokeAsync((ExpertiseTable[idx], idx));
        //}
        //private void LevelChanged(string val, int idx)
        //{
        //    ExpertiseTable[idx].ILULevelId = val;
        //    Upd.InvokeAsync((ExpertiseTable[idx], idx));
        //}

        private void Delete(int index)
        {
            //KnowledgeTable.RemoveAt(index);
            Del.InvokeAsync(index);
        }

        private void AddHere()
        {
            ILURegister newILU = new ILURegister();
            //KnowledgeTable.Add(niu);
            Add.InvokeAsync(newILU);
        }

        private void updateILULevel(int auxIluLevelId)
        {
            _newIlu.ILULevelId = auxIluLevelId;
        }

        private List<ILURegister> GetGroupedExpertiseTable()
        {
            return ExpertiseTable
                .Where(e => e.isActive)
                .GroupBy(e => new { e.DistributionId, ILUCategory = GetILUCategory(e.ILULevel?.ILULevelCode) })
                .Select(g => g.OrderByDescending(e => e.AcquisitionDate).First())
                .ToList();
        }

        private string GetILUCategory(string? iluLevelCode)
        {
            return iluLevelCode switch
            {
                "ITrainee" or "I" or "ILeader" or "LTrainee" or "LTraineeLeader" => "IGroup",
                "L" or "LLeader" or "UTrainee" or "ULeaderTrainee" => "LGroup",
                "U" or "ULeader" => "UGroup",
                _ => "Other"
            };
        }
    }
}