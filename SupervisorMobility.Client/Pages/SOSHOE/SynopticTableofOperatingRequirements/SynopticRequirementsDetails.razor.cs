using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.SOS_Process;
using System.Globalization;

namespace SupervisorMobility.Client.Pages.SOSHOE.SynopticTableofOperatingRequirements
{


    public partial class SynopticRequirementsDetails
    {
        [Parameter]
        public int? SynopticRequirementsId { get; set; }

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
        SOSSynopticTableofOperatingRequirements _sosSynopticRequeriments { get; set; } = new();
        SOSHub _soshub { get; set; } = new();
        Distribution _distribution { get; set; } = new();
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
                new BreadcrumbItem(text: Localizer["SynopticRequirements"], href: "/soshoe/SynopticRequirements"),
                new BreadcrumbItem(text: Localizer["SynopticRequirementsDetails"], href: "/soshoe/SynopticRequirements", disabled:true)
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
                _sosSynopticRequeriments = await SynopticRequirementsService.GetSOSSynopticTableofOperatingRequirements((int)SynopticRequirementsId, true, true, true);
                _soshub = await sosHubService.GetSOSHub((int)_sosSynopticRequeriments.SOSHubId, true, true, includePeople: true, includeInformation: true, includeModel: true);
                _distribution = await DistributionService.GetDistributionWithCollections((int)_soshub.PlantId, (int)_soshub.AreaId, (int)_soshub.DistributionId);
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
            NavigationManager.NavigateTo($"soshoe/SynopticRequirements/Update/{SynopticId}");
        }



        #endregion

        #region SynopticRequirementsLogbook

        public bool TryGetSynopticRequirementsLogbooksElementAtIndex(int index, out SOSSynopticRequirementsLogbook? item)
        {
            item = null;
            if (_sosSynopticRequeriments.SynopticRequirementsLogbooks == null || _sosSynopticRequeriments.SynopticRequirementsLogbooks.Count == 0)
            {
                return false;
            }

            int invertedIndex = _sosSynopticRequeriments.SynopticRequirementsLogbooks.Count - 1 - index;

            if (invertedIndex >= 0 && invertedIndex < _sosSynopticRequeriments.SynopticRequirementsLogbooks.Count)
            {
                item = _sosSynopticRequeriments?.SynopticRequirementsLogbooks?.ElementAt(invertedIndex);
                return true;
            }

            return false;
        }
        #endregion

        #region Hoe
        private void HoeDetails(int HoeId)
        {
            NavigationManager.NavigateTo($"/soshoe/Hub/Details/{HoeId}");
        }
        #endregion

        #region generalfunctions
        //&===================== FUNCTIONS FOR GENERAL COMPONENT =====================&\\

        /// <summary>
        /// Formats a nullable <see cref="DateTime"/> as "MONTH YEAR" in uppercase.
        /// Returns empty string if null.
        /// </summary>
        /// <param name="date">The date to format.</param>
        /// <returns>Formatted month and year string, or empty if null.</returns>
        private string DateFormat(DateTime? date)
        {
            if (!date.HasValue) return "";

            string language = CultureInfo.CurrentCulture.Name ?? "es-MX";
            CultureInfo cultureInfo = new CultureInfo(language);

            return date.Value.ToString("dd/MM/yyyy hh:mm:ss tt", cultureInfo).ToUpper();
        }


        #endregion

        #region downloadFormat
        //&===================== FUNCTIONS FOR DOWNLOAD FORMAT =====================&\\
        private async void DownloadSTOR()
        {
            await SynopticRequirementsService.GenerateExcelSTOperatingRequirements((int)SynopticRequirementsId);
        }

        #endregion

    }
}