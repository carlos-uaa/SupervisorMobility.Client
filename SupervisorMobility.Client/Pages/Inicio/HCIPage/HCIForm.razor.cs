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
using Blazorise.Utilities;

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
        public List<ILURegister> expertise = new();
        public List<HCICategory> categories = new();
        public List<Commentary> comments = new();
        private List<UserCourse> courses = new();
        public List<LocalUserCourses> newCourses = new();

        private List<Area> _areas = new();


        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        private async Task GetUserAsync()
        {
            if (!await TryGetAsync())
            {
                user = new();
            }
        }

        private async Task<bool> TryGetAsync()
        {
            bool hasProperty = await HasPropertyAsync();
            if (hasProperty)
            {
                json = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "user");
                user = JsonSerializer.Deserialize<User>(json) ?? new();

            }
            return hasProperty;
        }

        private async Task<bool> HasPropertyAsync()
            => await JSRuntime.InvokeAsync<bool>("localStorage.hasOwnProperty", "user");


        protected async override Task OnInitializedAsync()
        {
            var currentUrl = NavManager.Uri;
            Details = currentUrl.Contains("Details", StringComparison.OrdinalIgnoreCase);

            logged = await HasPropertyAsync();
            if (!logged)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error You have to log in", Severity.Error);
                NavManager.NavigateTo($"/");
                return;
            }

            await GetUserAsync();

            if (HCIID == null)
            {
                if(userId != 0)
                {
                    _hci.User = await UsrsService.GetUserAndCollection(userId);
                    _hci.UserId = userId;
                    _hci.Transactions = new List<HCITransaction>();
                    _hci.Categories = new List<HCICategory>();
                    _hci.Commentaries = new List<Commentary>();
                    _hci.CareerPaths = new List<UserCareerPath>();
                    _hci.Courses = new List<LocalUserCourses>();
                    if (_hci.User.ILURegisers.Any())
                        expertise = _hci.ILUs = _hci.User.ILURegisers.ToList();
                    else
                        _hci.ILUs = new List<ILURegister>();
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
                newCourses = _hci.Courses ?? new();
            }

            if(user != null && !string.IsNullOrEmpty(_hci.User?.Payroll.ToString()))
            {
                var coursesResponse = await UserCoursesService.GetUserCoursesAsync(_hci.User?.Payroll.ToString());
                if (coursesResponse.Success)
                    courses = coursesResponse.Data;
                else
                {
                    Snackbar.Add($"Error loading courses: {coursesResponse.Message}", Severity.Error);
                    courses = new List<UserCourse>();
                }    
            }

            var areasIds = expertise.Where(e => e.Distribution != null).Select(e => e.Distribution.AreaId).Distinct().ToList();
            if(areasIds != null && areasIds.Count > 0)
                _areas = await AreaServices.GetAreasByIds(areasIds);

            GetCareerPath();


            dataloaded = true;
        }

        private void GetCareerPath()
        {
            if(expertise == null || expertise.Count == 0)
                return;

            int careerPathNo = 0;

            foreach (var career in expertise)
            {
                careerPathNo++;
                if (careers.Any(c => c.CareerPathNo == careerPathNo))
                    continue;
                careers.Add(
                    new UserCareerPath
                    {
                        CareerPathNo = careerPathNo,
                        ChangeDate = career.AcquisitionDate,
                        EndDate = career.EndDate,
                        Department = _areas.FirstOrDefault(a => a.AreaId == career.Distribution?.AreaId)?.Code ?? "",
                        Process = career.Distribution?.Code ?? "",
                        OperationDescription = career.Distribution?.Description ?? "",
                        IsActive = true
                    }
                );
            }
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
            _hci.Courses = newCourses;

            if (HCIID == null)
            {
                _hci.ILUs = new();
                if (await HCIService.CreateHCI(_hci))
                    Snackbar.Add("Created succesfully", Severity.Success);
                else
                    Snackbar.Add("Error", Severity.Error);
            }
            else
                if(await HCIService.UpdateHCI(_hci))
                    Snackbar.Add("Updated succesfully", Severity.Success);
                else
                    Snackbar.Add("Error", Severity.Error);

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
                careers.RemoveAt(idx);
            else
                careers[idx].IsActive = false;
            if (!careers.Where(p => p.IsActive == true).Any())
                careers.Add(new UserCareerPath { CareerPathNo = 1, IsActive = true });
        }
        public async void ModifyCareerEntry(UserCareerPath krir, int idx)
        {
            careers[idx] = krir;
        }
        //----------------------

        //Expertise Control
        public async void AddExpert(ILURegister reg)
        {
            expertise.Add(reg);
        }
        public async void RemoveExpert(int idx)
        {
            if (expertise[idx] != null || (expertise[idx].ILURegisterid != null && expertise[idx].ILURegisterid != 0))
            {
                Snackbar.Add(Localizer["RemExpertice"], Severity.Warning);
                return;
            }

            if (expertise[idx].ILURegisterid == 0)
            {
                expertise.RemoveAt(idx);
            }
            else
            {
                expertise[idx].isActive = false;
            }
            if (!expertise.Where(p => p.isActive == true).Any())
                expertise.Add(new ILURegister { isActive = true });
        }
        public async void ModifyExpertEntry(ILURegister reg, int idx)
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
            categories.RemoveAll(p=>p.ChosenCategoryDepartmentId == 0 && p.HCICategoryId == 0);
            comments.RemoveAll(p => p.Comment.IsNullOrEmpty() && p.CommentaryId == 0);
        }

        private async void DownloadExcel()
        {
            await Exportation.ExportHCIToExcel(_hci.HCIId);
        }

        public async void addCourse(LocalUserCourses course)
        {
            if (course == null)
                return;

            newCourses.Add(course);
            Snackbar.Add("Course added", Severity.Info);
            newCourses = newCourses.ToList();
            StateHasChanged();
        }

        public async void removeCourse(LocalUserCourses course)
        {
            newCourses.Remove(course);
            Snackbar.Add("Course removed", Severity.Info);
            newCourses = newCourses.ToList();
            StateHasChanged();
        }
    }
}