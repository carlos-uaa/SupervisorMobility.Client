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
using Blazorise.Extensions;
using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Pages.Inicio.HCIPage
{
    public partial class HCIForm
    {
        [Parameter]
        public int? HCIID { get; set; }

        [Parameter]
        [SupplyParameterFromQuery(Name = "user")]
        public int userId { get; set; }

        public Data.Entities.HCI _hci = new ();
        public bool Details = false;
        public bool dataloaded = false;

        public List<HCITransaction> manualCap = new();
        public List<HCITransaction> companyCap = new();
        public List<HCITransaction> titles = new();
        public List<UserCareerPath> careers = new();
        public List<HCIILU> expertise = new();
        public List<HCICategory> categories = new();
        public List<Commentary> comments = new();

        protected async override Task OnInitializedAsync()
        {
            var currentUrl = NavManager.Uri;
            Details = currentUrl.Contains("Details", StringComparison.OrdinalIgnoreCase);

            if (HCIID == null)
            {
                if(userId != 0)
                {
                    _hci.User = await UsrsService.GetUser(userId);
                    _hci.UserId = userId;
                    _hci.Transactions = new List<HCITransaction>();
                    _hci.Categories = new List<HCICategory>();
                    _hci.Commentaries = new List<Commentary>();
                    _hci.CareerPaths = new List<UserCareerPath>();
                    _hci.ILUs = new List<HCIILU>();
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
                careers = _hci.CareerPaths ?? new();
                expertise = _hci.ILUs ?? new();
                categories = _hci.Categories ?? new();
                comments = _hci.Commentaries ?? new();
            }

            dataloaded = true;
        }

        private async void Complete()
        {
            CleanLists();
            _hci.Transactions = manualCap;
            _hci.Transactions.AddRange(companyCap);
            _hci.Transactions.AddRange(titles);
            _hci.CareerPaths = careers;
            _hci.ILUs = expertise;
            _hci.Categories = categories;
            _hci.Commentaries = comments;

            if (HCIID == null)
            {
                if (await HCIService.CreateHCI(_hci))
                {
                    Snackbar.Add("Created succesfully", Severity.Success);
                }
                else
                {
                    Snackbar.Add("Error", Severity.Error);
                }
            }
            else
            {
                if(await HCIService.UpdateHCI(_hci))
                {
                    Snackbar.Add("Updated succesfully", Severity.Success);
                }
                else
                {
                    Snackbar.Add("Error", Severity.Error);
                }
            }
            NavManager.NavigateTo("/HCI");
        }

        //Knowledge & Qualification Control
        public async void AddManual(HCITransaction trs)
        {
            manualCap.Add(trs);
        }
        public async void RemoveManual(int idx)
        {
            if (manualCap[idx].HCITransactionId == 0)
            {
                manualCap.RemoveAt(idx);  
            }
            else
            {
                manualCap[idx].IsActive = false;
            }
            if (!manualCap.Where(p => p.IsActive == true).Any())
                manualCap.Add(new HCITransaction { Type = 1, IsActive = true });
        }
        public async void ModifyManualEntry(HCITransaction trs, int idx)
        {
            manualCap[idx] = trs;
        }

        public async void AddCompany(HCITransaction trs)
        {
            companyCap.Add(trs);
        }
        public async void RemoveCompany(int idx)
        {
            if (companyCap[idx].HCITransactionId == 0)
            {
                companyCap.RemoveAt(idx);
            }
            else
            {
                companyCap[idx].IsActive = false;
            }
            if (!companyCap.Where(p => p.IsActive == true).Any())
                companyCap.Add(new HCITransaction { Type = 2, IsActive = true });
        }
        public async void ModifyCompanyEntry(HCITransaction trs, int idx)
        {
            companyCap[idx] = trs;
        }

        public async void AddQualif(HCITransaction trs)
        {
            titles.Add(trs);
        }
        public async void RemoveQualif(int idx)
        {
            if (titles[idx].HCITransactionId == 0)
            {
                titles.RemoveAt(idx);
            }
            else
            {
                titles[idx].IsActive = false;
            }
            if (!titles.Where(p => p.IsActive == true).Any())
                titles.Add(new HCITransaction { Type = 3, IsActive = true });
        }
        public async void ModifyQualifEntry(HCITransaction trs, int idx)
        {
            titles[idx] = trs;
        }
        //----------------------

        //Trajectory Control
        public async void AddCareer(UserCareerPath krir)
        {
            careers.Add(krir);
        }
        public async void RemoveCareer(int idx)
        {
            if (careers[idx].UserCareerPathId == 0)
            {
                careers.RemoveAt(idx);
            }
            else
            {
                careers[idx].IsActive = false;
            }
            if (!careers.Where(p => p.IsActive == true).Any())
                careers.Add(new UserCareerPath { CareerPathNo = 1, IsActive = true });
        }
        public async void ModifyCareerEntry(UserCareerPath krir, int idx)
        {
            careers[idx] = krir;
        }
        //----------------------

        //Expertise Control
        public async void AddExpert(HCIILU reg)
        {
            expertise.Add(reg);
        }
        public async void RemoveExpert(int idx)
        {
            if (expertise[idx].Register != null || (expertise[idx].RegisterILURegisterid != null && expertise[idx].RegisterILURegisterid != 0))
            {
                Snackbar.Add(Localizer["RemExpertice"], Severity.Warning);
                return;
            }

            if (expertise[idx].ID == 0)
            {
                expertise.RemoveAt(idx);
            }
            else
            {
                expertise[idx].IsActive = false;
            }
            if (!expertise.Where(p => p.IsActive == true).Any())
                expertise.Add(new HCIILU { IsActive = true });
        }
        public async void ModifyExpertEntry(HCIILU reg, int idx)
        {
            expertise[idx] = reg;
        }
        //----------------------

        //Category Control
        public async void AddCateg(HCICategory cat)
        {
            categories.Add(cat);
        }
        public async void RemoveCateg(int idx)
        {
            if (categories[idx].HCICategoryId == 0)
            {
                categories.RemoveAt(idx);
            }
            else
            {
                categories[idx].IsActive = false;
            }
            if (!categories.Where(p => p.IsActive == true).Any())
                categories.Add(new HCICategory());
        }
        public async void ModifyCategEntry(HCICategory cat, int idx)
        {
            categories[idx] = cat;
        }
        //----------------------

        //Notes Control
        public async void AddNote(Commentary com)
        {
            comments.Add(com);
        }
        public async void RemoveNote(int idx)
        {
            if (comments[idx].CommentaryId == 0)
            {
                comments.RemoveAt(idx);
            }
            else
            {
                comments[idx].IsActive = false;
            }
            if (!comments.Where(p => p.IsActive == true).Any())
                comments.Add(new Commentary());
        }
        public async void ModifyNoteEntry(Commentary cat, int idx)
        {
            comments[idx] = cat;
        }
        //----------------------

        private void CleanLists()
        {
            manualCap.RemoveAll(p => p.Description.IsNullOrEmpty() && p.HCITransactionId == 0);
            companyCap.RemoveAll(p=>p.Description.IsNullOrEmpty() && p.HCITransactionId == 0);
            titles.RemoveAll(p => p.Description.IsNullOrEmpty() && p.HCITransactionId == 0);

            careers.RemoveAll(p=>p.OperationDescription.IsNullOrEmpty() && p.UserCareerPathId == 0);
            //Add expertise validations
            //for (int i = 0; i < expertise.Count; i++)
            //{
            //    if (checkExpertise(expertise[i])) expertise.RemoveAt(i);
            //}
            expertise.RemoveAll(p=>p.Description.IsNullOrEmpty() && p.ID == 0);
            categories.RemoveAll(p=>p.ChosenCategoryDepartmentId == 0 && p.HCICategoryId == 0);
            comments.RemoveAll(p => p.Comment.IsNullOrEmpty() && p.CommentaryId == 0);
        }


        private async void DownloadExcel()
        {
            await Exportation.ExportHCIToExcel(_hci.HCIId);
        }
    }
}