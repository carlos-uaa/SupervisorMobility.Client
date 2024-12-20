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
                    _hci.Comments = new List<Commentary>();
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
                comments = _hci.Comments ?? new();
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
            _hci.Comments = comments;

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






        public string testimage2 = "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/4QBpRXhpZgAASUkqAAgAAAABAA4BAgBHAAAAGgAAAAAAAABEZWZhdWx0IHByb2ZpbGUgcGljdHVyZSwgYXZhdGFyLCBwaG90byBwbGFjZWhvbGRlci4gVmVjdG9yIGlsbHVzdHJhdGlvbv/hBWtodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+Cjx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iPgoJPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4KCQk8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIiB4bWxuczpwaG90b3Nob3A9Imh0dHA6Ly9ucy5hZG9iZS5jb20vcGhvdG9zaG9wLzEuMC8iIHhtbG5zOklwdGM0eG1wQ29yZT0iaHR0cDovL2lwdGMub3JnL3N0ZC9JcHRjNHhtcENvcmUvMS4wL3htbG5zLyIgICB4bWxuczpHZXR0eUltYWdlc0dJRlQ9Imh0dHA6Ly94bXAuZ2V0dHlpbWFnZXMuY29tL2dpZnQvMS4wLyIgeG1sbnM6ZGM9Imh0dHA6Ly9wdXJsLm9yZy9kYy9lbGVtZW50cy8xLjEvIiB4bWxuczpwbHVzPSJodHRwOi8vbnMudXNlcGx1cy5vcmcvbGRmL3htcC8xLjAvIiAgeG1sbnM6aXB0Y0V4dD0iaHR0cDovL2lwdGMub3JnL3N0ZC9JcHRjNHhtcEV4dC8yMDA4LTAyLTI5LyIgeG1sbnM6eG1wUmlnaHRzPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvcmlnaHRzLyIgcGhvdG9zaG9wOkNyZWRpdD0iR2V0dHkgSW1hZ2VzL2lTdG9ja3Bob3RvIiBHZXR0eUltYWdlc0dJRlQ6QXNzZXRJRD0iMTIyMzY3MTM5MiIgeG1wUmlnaHRzOldlYlN0YXRlbWVudD0iaHR0cHM6Ly93d3cuaXN0b2NrcGhvdG8uY29tL2xlZ2FsL2xpY2Vuc2UtYWdyZWVtZW50P3V0bV9tZWRpdW09b3JnYW5pYyZhbXA7dXRtX3NvdXJjZT1nb29nbGUmYW1wO3V0bV9jYW1wYWlnbj1pcHRjdXJsIiA+CjxkYzpjcmVhdG9yPjxyZGY6U2VxPjxyZGY6bGk+Y3VtYWNyZWF0aXZlPC9yZGY6bGk+PC9yZGY6U2VxPjwvZGM6Y3JlYXRvcj48ZGM6ZGVzY3JpcHRpb24+PHJkZjpBbHQ+PHJkZjpsaSB4bWw6bGFuZz0ieC1kZWZhdWx0Ij5EZWZhdWx0IHByb2ZpbGUgcGljdHVyZSwgYXZhdGFyLCBwaG90byBwbGFjZWhvbGRlci4gVmVjdG9yIGlsbHVzdHJhdGlvbjwvcmRmOmxpPjwvcmRmOkFsdD48L2RjOmRlc2NyaXB0aW9uPgo8cGx1czpMaWNlbnNvcj48cmRmOlNlcT48cmRmOmxpIHJkZjpwYXJzZVR5cGU9J1Jlc291cmNlJz48cGx1czpMaWNlbnNvclVSTD5odHRwczovL3d3dy5pc3RvY2twaG90by5jb20vcGhvdG8vbGljZW5zZS1nbTEyMjM2NzEzOTItP3V0bV9tZWRpdW09b3JnYW5pYyZhbXA7dXRtX3NvdXJjZT1nb29nbGUmYW1wO3V0bV9jYW1wYWlnbj1pcHRjdXJsPC9wbHVzOkxpY2Vuc29yVVJMPjwvcmRmOmxpPjwvcmRmOlNlcT48L3BsdXM6TGljZW5zb3I+CgkJPC9yZGY6RGVzY3JpcHRpb24+Cgk8L3JkZjpSREY+CjwveDp4bXBtZXRhPgo8P3hwYWNrZXQgZW5kPSJ3Ij8+Cv/tAJZQaG90b3Nob3AgMy4wADhCSU0EBAAAAAAAehwCUAAMY3VtYWNyZWF0aXZlHAJ4AEdEZWZhdWx0IHByb2ZpbGUgcGljdHVyZSwgYXZhdGFyLCBwaG90byBwbGFjZWhvbGRlci4gVmVjdG9yIGlsbHVzdHJhdGlvbhwCbgAYR2V0dHkgSW1hZ2VzL2lTdG9ja3Bob3Rv/9sAQwANCQoLCggNCwoLDg4NDxMgFRMSEhMnHB4XIC4pMTAuKS0sMzpKPjM2RjcsLUBXQUZMTlJTUjI+WmFaUGBKUVJP/9sAQwEODg4TERMmFRUmTzUtNU9PT09PT09PT09PT09PT09PT09PT09PT09PT09PT09PT09PT09PT09PT09P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";
    }
}