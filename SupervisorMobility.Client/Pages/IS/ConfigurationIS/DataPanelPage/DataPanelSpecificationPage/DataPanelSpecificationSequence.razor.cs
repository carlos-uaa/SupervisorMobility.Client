using MudBlazor;
using MudBlazor.Utilities;
using Microsoft.JSInterop;
namespace SupervisorMobility.Client.Pages.IS.ConfigurationIS.DataPanelPage.DataPanelSpecificationPage
{
    public partial class DataPanelSpecificationSequence
    {
        // Parameters
        [Parameter]

        public int DataPanelId { get; set; }
    

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>();

        // Objects
        DataPanel _DataPanel = new();
        DataPanelSpecification _DataPanelSpecification = new();

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        string currentLanguage = "es-ES";

        //Loading Elements
        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };

        public bool ShowLoading = true;

        // Initialization
        protected override async Task OnParametersSetAsync()
        {
         
            _links = new List<BreadcrumbItem>
                     {
                         new BreadcrumbItem(text: Localizer["home"], href: "/"),
                         new BreadcrumbItem(text: Localizer["configurationIS"], href: "/configurationIS"),
                         new BreadcrumbItem(text: Localizer["DataPanels"], href: $"/configurationIS/DataPanels"),
                     };

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

                _DataPanel = await DataPanelServices.GetDataPanel(DataPanelId, true);
                _DataPanel.Specifications = _DataPanel.Specifications.OrderBy(dp => dp.ItemOrder).ToList();
                _links.Add(new BreadcrumbItem(text: _DataPanel.DataTitle, href: $"/configurationIS/DataPanels/Details/{DataPanelId}"));
                _links.Add(new BreadcrumbItem(text: Localizer["DataSpecificationSequence"], href: $"/configurationIS/DataPanels/Details/{DataPanelId}", disabled: true));


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


        void UpdateCategory(int categoryId)
        {
            NavigationManager.NavigateTo($"configurationIS/DataPanels/Update/{categoryId}");
        }
        void CancelPage()
        {
            NavigationManager.NavigateTo($"/configurationIS/DataPanels/Details/{DataPanelId}");
        }


        // Drag and drop category
        async void ItemUpdated(MudItemDropInfo<DataPanelSpecification> dropItem)
        {

            var indexOffset = 2;

            dropItem.Item.Container = dropItem.DropzoneIdentifier;

            _DataPanel.Specifications.UpdateOrder(dropItem, item => item.ItemOrder, indexOffset);

            int currentspecification = dropItem.Item.DataPanelSpecificationId;
            int newSequence = dropItem.IndexInZone + 1;

            DataPanelSpecification dbCategory = await DataPanelServices.GetDataPanelSpecification(currentspecification);
            _DataPanelSpecification = dbCategory;
            _DataPanelSpecification.ItemOrder = newSequence;

            UpdateSequence(currentspecification, _DataPanelSpecification);

            base.StateHasChanged();
        }

        // Update sequence
        void UpdateSequence(int currentCategory, DataPanelSpecification specificationItem)
        {
            DataPanelServices.UpdateSpecificationSequence(currentCategory, specificationItem);
        }

    }
}