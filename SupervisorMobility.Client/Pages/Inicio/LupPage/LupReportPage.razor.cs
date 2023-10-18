using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Services.LupService;

namespace SupervisorMobility.Client.Pages.Inicio.LupPage
{
    public partial class LupReportPage
    {

        public List<Lup> lupList { get; set; }

        public List<Lup> _lup { get; set; } = new();

        private bool visible = false;
        private int lupId;
        private int year;
        private int operationId;

        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true, DisableBackdropClick = true, CloseButton = true };

        private List<BreadcrumbItem> _links;

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        // Initialization
        protected async override Task OnInitializedAsync()
        {

            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "#"),
                new BreadcrumbItem("LUP", href: "/lup"),
                new BreadcrumbItem(text: Localizer["lupReport"], href: "/lupReport", disabled: true)
            };

            logged = await HasPropertyAsync();
            if (!logged)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add(Localizer["errorYouHaveToLogIn"], Severity.Warning);
                NavigationManager.NavigateTo($"/");
            }
            else
            {

                await GetUserAsync();


                if(user != null)
                {
                    lupList = await LupServices.GetAllLup();
                    _lup = lupList;
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

        // Delete lup
        async Task DeleteLup(int lupId)
        {
            _lup.RemoveAll(l => l.LupId == lupId);
            await LupServices.DeleteLup(lupId);

            _lup = await LupServices.GetAllLup();

            visibleDelete = false;

        }

        // Update product
        void EditLup(int lupId)
        {
            NavigationManager.NavigateTo($"lup/updatelup/{lupId}");
        }

        private string searchString = "";

        private bool FilterFunc(Lup element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.LupId.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Status.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Oportunity.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{element.LupId} {element.Status} {element.Observer}".Contains(searchString))
                return true;
            return false;
        }

        //Modal

        private void OpenDialog2(int id)
        {
            lupId = id;
            visible = true;
        }
        void Close() => visible = false;



        //Delete lup
        private bool visibleDelete = false;
        public int deleteLupId = 0;
        private void OpenDeleteDialog(int deleteId)
        {
            deleteLupId = deleteId;
            visibleDelete = true;
        }
        void CloseDeleteModal() => visibleDelete = false;
        private DialogOptions dialogDeleteOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, Position = DialogPosition.TopCenter, DisableBackdropClick = true, CloseButton = true };


        //Double clic go to details
        private DateTime lastTouchTime = DateTime.MinValue;
        private readonly TimeSpan doubleTouchInterval = TimeSpan.FromMilliseconds(300);

        private void HandleTouchStart(int jobObsId)
        {
            DateTime now = DateTime.Now;
            TimeSpan timeSinceLastTouch = now - lastTouchTime;

            if (timeSinceLastTouch < doubleTouchInterval)
            {
                OpenDialog2(jobObsId);
            }

            lastTouchTime = now;
        }

        private int selectedRowNumber = -1;
        private MudTable<Lup> SelectTableEvent;

        private void RowClickEvent(TableRowClickEventArgs<Lup> tableRowClickEventArgs)
        {
        }


        private string SelectedRowClassFunc(Lup element, int rowNumber)
        {
            if (selectedRowNumber == rowNumber)
            {
                return string.Empty;
            }
            else if (SelectTableEvent.SelectedItem != null && SelectTableEvent.SelectedItem.Equals(element))
            {
                selectedRowNumber = rowNumber;
                return "selected";
            }
            else
            {
                return string.Empty;
            }
        }

        int totalNew = 0;
        int totalInProgress = 0;
        int totalCanceled = 0;
        int totalFinished = 0;


        private async void Filters()
        {
            if(year != 0 && operationId != 0)
            {
                _lup = await LupServices.GetLupsByFilters(year, operationId);
            }
            else if(year == 0 &&  operationId == 0) 
            {
                _lup = lupList;
            }

            totalNew = _lup.Where(l => l.Status == 1 && l.IsActive == true).ToList().Count();
            totalInProgress = _lup.Where(l => l.Status == 2 && l.IsActive == true).ToList().Count();
            totalCanceled = _lup.Where(l => l.Status == 3 && l.IsActive == true).ToList().Count();
            totalFinished = _lup.Where(l => l.Status == 4 && l.IsActive == true).ToList().Count();
            StateHasChanged();
        }


    }
}
