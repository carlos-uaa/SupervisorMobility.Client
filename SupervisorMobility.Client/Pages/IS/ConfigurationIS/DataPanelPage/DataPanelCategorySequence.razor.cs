using MudBlazor;
using MudBlazor.Utilities;

namespace SupervisorMobility.Client.Pages.IS.ConfigurationIS.DataPanelPage
{
    public partial class DataPanelCategorySequence
    {

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>();
      
  
        // Object
        public List<DataPanel> _datapanelCategories { get; set; } = new();
        public DataPanel _datapanel { get; set; } = new();
        
        //loading
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
                new BreadcrumbItem(text: Localizer["DataPanel"], href: "/configurationIS/DataPanels"),
                new BreadcrumbItem("Sequence", href: "", disabled: true),
            };

            BreadcrumbService.UpdateBreadcrumbs(_links);

            _datapanelCategories = await DataPanelServices.GetAllDataPanels();

            _datapanelCategories = _datapanelCategories.OrderBy(dp => dp.ItemOrder).ToList();

        }

        // Drag and drop category
        async void ItemUpdated(MudItemDropInfo<DataPanel> dropItem)
        {

                var indexOffset = 2;

                dropItem.Item.Container = dropItem.DropzoneIdentifier;

                _datapanelCategories.UpdateOrder(dropItem, item => item.ItemOrder, indexOffset);

                int currentCategory = dropItem.Item.DataPanelId;
                int newSequence = dropItem.IndexInZone + 1;

                DataPanel dbCategory = await DataPanelServices.GetDataPanel(currentCategory);
                _datapanel = dbCategory;
                _datapanel.ItemOrder = newSequence;

                UpdateSequence(currentCategory, _datapanel);
            
               base.StateHasChanged();
        }

        // Update sequence
        void UpdateSequence(int currentCategory, DataPanel checklistCategory)
        {
            DataPanelServices.UpdatePanelSequence(currentCategory, checklistCategory);
        }
    }
}
