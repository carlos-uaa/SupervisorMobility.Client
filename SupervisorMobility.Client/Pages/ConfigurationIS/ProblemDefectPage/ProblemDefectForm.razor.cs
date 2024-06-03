using BlazorCameraStreamer;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.Client.Pages.ConfigurationIS.ProblemDefectPage
{
    public partial class ProblemDefectForm
    {
        [Parameter]
        public int? ProblemDefectId { get; set; }

        public ProblemDefect _ProblemDefect{ get; set; } = new ProblemDefect();

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>();

        //Loading
        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };
        public bool ShowLoading = true;


        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        public enum PageType
        {
            Details,
            Create,
            Update,
            Another
        }

        public PageType pageType { get; set; }

        

        // Initialization
        protected async override Task OnInitializedAsync()
        {
            var currentUrl = NavigationManager.Uri;

            pageType = currentUrl.Contains("Details", StringComparison.OrdinalIgnoreCase) ? PageType.Details : PageType.Another;

            if (pageType == PageType.Another)
            {
                pageType = currentUrl.Contains("Create", StringComparison.OrdinalIgnoreCase) ? PageType.Create : PageType.Another;
            }

            if (pageType == PageType.Another)
            {
                pageType = currentUrl.Contains("Update", StringComparison.OrdinalIgnoreCase) ? PageType.Update : PageType.Another;
            }
            

            _sourceMsgLoading.Add($"{Localizer1["Loading1"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading2"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading3"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading4"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading5"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading6"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading7"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading8"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading9"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading10"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading11"]}");


            _links = new List<BreadcrumbItem>
             {
                new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem(text: Localizer["configurationIS"], href: "/configurationIS"),
                new BreadcrumbItem(text: Localizer["ProblemDefects"], href: "/configurationIS/ProblemDefects")
            };


            await GetUserAsync();
            logged = await HasPropertyAsync();
            if (!logged)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error You have to log in", Severity.Error);
                NavigationManager.NavigateTo($"/");
            }
            else
            {
                try
                {
                   
                    switch (pageType)
                    {
                        case PageType.Details:
                            if (ProblemDefectId != null)
                            {
                                _ProblemDefect = await ProblemDefectsServices.GetProblemDefect((int)ProblemDefectId);
                                _links.Add(new BreadcrumbItem(text: Localizer["Details"], href: $"/configurationIS/ProblemDefects/{ProblemDefectId}", disabled: true));
                                _links.Add(new BreadcrumbItem(text: _ProblemDefect.DefectDescription, href: $"/configurationIS/ProblemDefects/{ProblemDefectId}", disabled: true));
                            }
                            break;
                        case PageType.Create:
                            _links.Add(new BreadcrumbItem(text: Localizer["Create"], href: $"/configurationIS/ProblemDefects/", disabled: true));
                            _ProblemDefect.IsActive = true;
                            break;

                        case PageType.Update:
                            if (ProblemDefectId != null)
                            {
                                _ProblemDefect = await ProblemDefectsServices.GetProblemDefect((int)ProblemDefectId);
                                //_ProblemDefect 
                            _links.Add(new BreadcrumbItem(text: Localizer["Update"], href: $"/configurationIS/ProblemDefects/", disabled: true));
                                _links.Add(new BreadcrumbItem(text: _ProblemDefect.DefectDescription, href: $"/configurationIS/ProblemDefects/{ProblemDefectId}", disabled: true));
                            }
                            break;
                    }

                   

                }
                catch (Exception ex)
                {

                }
                finally
                {
                    BreadcrumbService.UpdateBreadcrumbs(_links);
                    ShowLoading = false;
                    base.StateHasChanged();
                }
            }



        }

        //Local storage user
        private async Task GetUserAsync()
        {
            if (!await TryGetAsync())
                user = new();
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

        private async void SubmitOperations()
        {
            switch (pageType)
            {
                case PageType.Create:

                    var resultCreate = await ProblemDefectsServices.CreateProblemDefect(_ProblemDefect);

                    if (resultCreate != null)
                    {
                        _ProblemDefect = resultCreate;
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Create Succes", Severity.Success);
                        NavigationManager.NavigateTo($"/configurationIS/ProblemDefect");

                    }
                    else
                    {
                        //suces create
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Create Fail", Severity.Error);
                        //NavigationManager.NavigateTo($"/");
                    }

                    break;

                case PageType.Update:
                    ProblemDefect? resultUpdate = await ProblemDefectsServices.UpdateProblemDefect(_ProblemDefect);

                    if (resultUpdate != null)
                    {
                     
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Update Succes", Severity.Success);
                        NavigationManager.NavigateTo($"/configurationIS/ProblemDefect");
                        //NavigationManager.NavigateTo($"/");
                    }
                    else
                    {
                        //suces create
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Update Fail", Severity.Error);
                        //NavigationManager.NavigateTo($"/");
                    }
                    break;
            }
        }

        void ProblemDefectUpdate(int ProblemDefectsId)
        {
            NavigationManager.NavigateTo($"configurationIS/ProblemDefect/Update/{ProblemDefectsId}", forceLoad: true);
        }

    }
}