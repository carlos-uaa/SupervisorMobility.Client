using System.Globalization;
using MudBlazor;
using Microsoft.JSInterop;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.SOS_Process;
using SupervisorMobility.Client.Services.SOS_Services.SOSHubService;

namespace SupervisorMobility.Client.Pages.SOSHOE.SynopticTableofControlPoints
{
    /// <summary>
    /// Code-behind for the Synoptic Table of Control Points update page.
    /// Loads the existing record, exposes editable fields (ProcessName, Creator,
    /// Reviewer, Approver) and persists changes via <see cref="ISynopticControlPointsService"/>.
    /// </summary>
    public partial class SynopticControlPointsUpdate
    {
        // ── Parameters ──────────────────────────────────────────────────────────
        [Parameter]
        public int? SynopticControlPointsId { get; set; }

        // ── Breadcrumb ───────────────────────────────────────────────────────────
        private List<BreadcrumbItem> _links = new();

        // ── Loading ──────────────────────────────────────────────────────────────
        public bool ShowLoading = true;
        public bool _saving = false;
        private IList<string> _sourceMsgLoading = new List<string>();

        // ── User login ───────────────────────────────────────────────────────────
        private string _json = string.Empty;
        public User user = new();
        public bool IsLoggedIn = false;

        // ── Domain data ──────────────────────────────────────────────────────────
        private SOSSynopticTableofControlPoints _record = new();
        private SOSHub _soshub = new();
        private List<User> _allUsers = new();

        // ════════════════════════════════════════════════════════════════════════
        //  INITIALIZATION
        // ════════════════════════════════════════════════════════════════════════

        protected override async Task OnInitializedAsync()
        {
            PopulateLoadingMessages();
            InitializeBreadcrumbs();

            if (!await CheckUserLoginAsync())
                return;

            await LoadDataAsync();

            ShowLoading = false;
            StateHasChanged();
        }

        // ── Init helpers ─────────────────────────────────────────────────────────

        private void PopulateLoadingMessages()
        {
            _sourceMsgLoading = Enumerable.Range(1, 11)
                .Select(i => Localizer1[$"Loading{i}"].Value)
                .ToList();
        }

        private void InitializeBreadcrumbs()
        {
            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(Localizer["homeSOSHOE"], href: "/soshoe"),
                new BreadcrumbItem(Localizer["SynopticControlPoints"], href: "/soshoe/SynopticControlPoints"),
                new BreadcrumbItem("Edit", href: null, disabled: true)
            };
            BreadcrumbService.UpdateBreadcrumbs(_links);
        }

        private async Task<bool> CheckUserLoginAsync()
        {
            IsLoggedIn = await JSRuntime.InvokeAsync<bool>("localStorage.hasOwnProperty", "user");
            if (!IsLoggedIn)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add("Error: You have to log in", Severity.Error);
                NavigationManager.NavigateTo("/");
            }
            return IsLoggedIn;
        }

        private async Task LoadDataAsync()
        {
            _record = await SynopticControlPointsService
                .GetSOSSynopticTableofControlPoints((int)SynopticControlPointsId!, true, true, true);

            _soshub = await sosHubService
                .GetSOSHub((int)_record.SOSHubId!, true, true,
                           includePeople: true, includeInformation: true, includeModel: true);

            _allUsers = await UsersService.GetUsers(includeCollections: false, includeSubordinates: false);
        }

        // ════════════════════════════════════════════════════════════════════════
        //  AUTOCOMPLETE
        // ════════════════════════════════════════════════════════════════════════

        private Task<IEnumerable<User>> SearchUsers(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return Task.FromResult(_allUsers.Take(20));

            var lower = text.ToLowerInvariant();
            var results = _allUsers
                .Where(u => u.Name != null && u.Name.ToLowerInvariant().Contains(lower))
                .Take(20);

            return Task.FromResult(results);
        }

        // ════════════════════════════════════════════════════════════════════════
        //  SAVE
        // ════════════════════════════════════════════════════════════════════════

        private async Task SaveChanges()
        {
            _saving = true;
            StateHasChanged();

            var updated = await SynopticControlPointsService
                .UpdateSOSSynopticTableofControlPoints(_record);

            _saving = false;

            if (updated != null)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add("Changes saved successfully.", Severity.Success);
                NavigationManager.NavigateTo($"/soshoe/SynopticControlPoints/Details/{_record.SOSSynopticTableofControlPointsId}");
            }
            else
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add("Error saving changes. Please try again.", Severity.Error);
            }

            StateHasChanged();
        }

        // ════════════════════════════════════════════════════════════════════════
        //  NAVIGATION
        // ════════════════════════════════════════════════════════════════════════

        private void GoBack()
            => NavigationManager.NavigateTo(
                $"/soshoe/SynopticControlPoints/Details/{_record.SOSSynopticTableofControlPointsId}");

        private void HoeDetails(int hoeId)
            => NavigationManager.NavigateTo($"/soshoe/hoe/HoeDetails/{hoeId}");

        // ════════════════════════════════════════════════════════════════════════
        //  LOGBOOK HELPER
        // ════════════════════════════════════════════════════════════════════════

        public bool TryGetLogbookAt(int index, out SOSSynopticPointsLogbook? item)
        {
            item = null;
            var books = _record.SynopticPointsLogbooks;
            if (books == null || books.Count == 0) return false;

            int invertedIndex = books.Count - 1 - index;
            if (invertedIndex < 0 || invertedIndex >= books.Count) return false;

            item = books.ElementAt(invertedIndex);
            return true;
        }

        // ════════════════════════════════════════════════════════════════════════
        //  DATE FORMAT HELPERS
        // ════════════════════════════════════════════════════════════════════════

        private static string DateFormatString(DateTime? date)
        {
            if (!date.HasValue) return string.Empty;
            var culture = new CultureInfo(CultureInfo.CurrentCulture.Name ?? "es-MX");
            return date.Value.ToString("MMMM yyyy", culture).ToUpper();
        }

        private static string DateFormat(DateTime? date)
        {
            if (!date.HasValue) return string.Empty;
            var culture = new CultureInfo(CultureInfo.CurrentCulture.Name ?? "es-MX");
            return date.Value.ToString("dd/MM/yyyy hh:mm:ss tt", culture).ToUpper();
        }
    }
}
