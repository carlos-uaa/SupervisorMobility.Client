using Microsoft.JSInterop;
using MudBlazor;

namespace SupervisorMobility.Client.Pages.ConfigurationIS.PartsPage
{
    public partial class PartIndex
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>();

        // Objects
        public List<Part> _Parts { get; set; } = new();

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        //Loading
        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };
        public bool ShowLoading = true;

        // Initialization
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
                new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem(text: Localizer["configurationIS"], href: "/configurationIS"),
                new BreadcrumbItem(text: Localizer["Parts"], href: "", disabled: true)
            };

            BreadcrumbService.UpdateBreadcrumbs(_links);

            try
            {
                await GetUserAsync();
                logged = await HasPropertyAsync();
                if (!logged)
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Error You have to log in", Severity.Error);
                    NavigationManager.NavigateTo($"/");
                }

                _Parts = await PartsServices.GetAllParts();

            }
            catch (Exception ex)
            {

            }
            finally
            {
                ShowLoading = false;
                base.StateHasChanged();
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


        // Create CreateParts
        void CreateParts()
        {
            NavigationManager.NavigateTo($"configurationIS/Parts/Create");
        }
        // Details CreateParts
        void PartsDetails(int PartsId)
        {
            NavigationManager.NavigateTo($"configurationIS/Parts/Details/{PartsId}");
        }

        // Update category
        void PartUpdate(int PartsId)
        {
            NavigationManager.NavigateTo($"configurationIS/Parts/Update/{PartsId}");
        }

        async Task DeleteParts(int PartsId)
        {
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this Parts?");

            if (confirm)
            {
                _Parts.RemoveAll(p => p.PartId == PartsId);
                await PartsServices.DeletePart(PartsId);
            }
        }

        private string searchString = "";

        private bool FilterFunc(Part element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;


            if (element.PartName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;

            if (element.PartNumber.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;

            var searchWords = searchString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Verificar si todas las palabras en searchWords están contenidas en DataTitle
            if (searchWords.All(word => element.PartNumber.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0))
                return true;


            // Verificar si todas las palabras en searchWords están contenidas en DataTitle
            if (searchWords.All(word => element.PartName.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0))
                return true;


            return false;
        }

        private int selectedRowNumber = -1;
        private MudTable<Part> SelectTableEvent;

        private void RowClickEvent(TableRowClickEventArgs<Part> tableRowClickEventArgs)
        {
        }

        private string SelectedRowClassFunc(Part element, int rowNumber)
        {
            if (selectedRowNumber == rowNumber)
            {
                selectedRowNumber = -1;
                if (SelectTableEvent.SelectedItem != null && SelectTableEvent.SelectedItem.Equals(element))
                {
                    PartsDetails(element.PartId);
                }
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



    }
}