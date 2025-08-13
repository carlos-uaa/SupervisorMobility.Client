using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.SOS_Process;
using SupervisorMobility.Client.Services.SOS_Services.SOSHubService;

namespace SupervisorMobility.Client.Pages.SOSHOE.SynopticTableofControlPoints
{


    public partial class SynopticControlPoints
    {
        [Parameter]
        public int? SynopticControlPointsId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links;

        //Loading
        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<MudBlazor.Color> _Colors = new List<MudBlazor.Color>() { MudBlazor.Color.Default, MudBlazor.Color.Primary, MudBlazor.Color.Secondary, MudBlazor.Color.Success, MudBlazor.Color.Info, MudBlazor.Color.Default, MudBlazor.Color.Primary, MudBlazor.Color.Secondary, MudBlazor.Color.Success, MudBlazor.Color.Info };
        public bool ShowLoading = true;

        //UserLogin
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        //SynopticRequirements
        SOSSynopticTableofControlPoints _sosSynopticControlPoints { get; set; } = new();
        SOSHub _soshub { get; set; } = new();
        protected async override Task OnInitializedAsync()
        {
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
                new BreadcrumbItem(text: Localizer["homeSOSHOE"], href: "/soshoe"),
                new BreadcrumbItem(text: Localizer["SynopticControlPoints"], href: "/soshoe/SynopticControlPoints"),
                new BreadcrumbItem(text: Localizer["SynopticControlPointsDetails"], href: "/soshoe/SynopticControlPoints", disabled:true)
            };

            BreadcrumbService.UpdateBreadcrumbs(_links);


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
                _sosSynopticControlPoints = await SynopticControlPointsService.GetSOSSynopticTableofControlPoints((int)SynopticControlPointsId, true, true, true);
                _soshub = await sosHubService.GetSOSHub((int)_sosSynopticControlPoints.SOSHubId, true, true, includePeople: true, includeInformation: true, includeModel: true);
            
            }
            ShowLoading = false;
            StateHasChanged();
        }

        #region UserLogin
        //Local storage user
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
                user = System.Text.Json.JsonSerializer.Deserialize<User>(json) ?? new();

            }
            return hasProperty;
        }

        private async Task<bool> HasPropertyAsync()
            => await JSRuntime.InvokeAsync<bool>("localStorage.hasOwnProperty", "user");


        #endregion

        #region SynopticRequirements
        private void UpdateSynopticRequirements(int SynopticId)
        {

        }



        #endregion

        #region SynopticRequirementsLogbook

        public bool TryGetSynopticPointsLogbooksElementAtIndex(int index, out SOSSynopticPointsLogbook? item)
        {
            item = null;
            if (_sosSynopticControlPoints.SynopticPointsLogbooks == null || _sosSynopticControlPoints.SynopticPointsLogbooks.Count == 0)
            {
                return false;
            }

            int invertedIndex = _sosSynopticControlPoints.SynopticPointsLogbooks.Count - 1 - index;

            if (invertedIndex >= 0 && invertedIndex < _sosSynopticControlPoints.SynopticPointsLogbooks.Count)
            {
                item = _sosSynopticControlPoints?.SynopticPointsLogbooks?.ElementAt(invertedIndex);
                return true;
            }

            return false;
        }
        #endregion

        #region Hoe
        private void HoeDetails(int HoeId)
        {
            NavigationManager.NavigateTo($"/soshoe/hoe/HoeDetails/{HoeId}");
        }
            #endregion

        }
}