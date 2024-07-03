using DocumentFormat.OpenXml.Office2010.PowerPoint;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazorFix;
using static MudBlazor.CategoryTypes;

namespace SupervisorMobility.Client.Pages.IS.ConfigurationIS.DataPanelPage
{
    public partial class DataPanelForm
    {
        [Parameter]

        public int? DataPanelId { get; set; }

        public DataPanel _datapanel { get; set; } = new DataPanel();

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
            Create,
            Update
        }

        public PageType pageType { get; set; }

        //Flags
        private bool AddSpecification = false;

        //Spectable
      
        private string searchString = "";
        private DataPanelSpecification selectedItem1 = null;
        private DataPanelSpecification elementBeforeEdit;
        private HashSet<DataPanelSpecification> selectedItems1 = new HashSet<DataPanelSpecification>();
        private TableApplyButtonPosition applyButtonPosition = TableApplyButtonPosition.Start;
        private TableEditButtonPosition editButtonPosition = TableEditButtonPosition.Start;
        private TableEditTrigger editTrigger = TableEditTrigger.RowClick;
       
        private IEnumerable<DataPanelSpecification> Elements = new List<DataPanelSpecification>();

      
        // Initialization
        protected async override Task OnInitializedAsync()
        {
            var currentUrl = NavigationManager.Uri;
            pageType = currentUrl.Contains("Create", StringComparison.OrdinalIgnoreCase) ? PageType.Create : PageType.Update;


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
                new BreadcrumbItem(text: Localizer["DataPanel"], href: "/configurationIS/DataPanels")
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
                        case PageType.Create:
                            _datapanel.IsActive = true;
                            break;

                        case PageType.Update:
                            if (DataPanelId != null)
                            {
                                _datapanel = await DataPanelServices.GetDataPanel((int)DataPanelId, true);
                                _links.Add(new BreadcrumbItem(text: _datapanel.DataTitle, href: $"/configurationIS/DataPanels/Details/{DataPanelId}", disabled: true));
                            }
                            break;
                    }

                    // _dataPanelsCategories = await DataPanelServices.GetAllDataPanels();

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

                    var resultCreate = await DataPanelServices.CreateDataPanel(_datapanel);

                    if(resultCreate != null)
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Create Succes", Severity.Success);
                        NavigationManager.NavigateTo($"/configurationIS/DataPanels");

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
                    DataPanel? resultUpdate = await DataPanelServices.UpdateDataPanel(_datapanel);

                    if (resultUpdate != null)
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Update Succes", Severity.Success);
                        NavigationManager.NavigateTo($"/configurationIS/DataPanels");
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


        private void AddOneMoreSpecification()
        {
            AddSpecification = true;
            DataPanelSpecification NewItemSpec = new DataPanelSpecification();

            if (_datapanel.Specifications == null)
            {
                _datapanel.Specifications = new List<DataPanelSpecification>();
            }

            NewItemSpec.DataSpecification = "New Item, pls change this";
            NewItemSpec.IsActive = true;

            _datapanel.Specifications?.Add(NewItemSpec);


            AddSpecification = false;
        }

        private bool FilterFunc(DataPanelSpecification element)
        {

            if (string.IsNullOrWhiteSpace(searchString))
                return true;
           

            return false;
        }

        private void BackupItem(object element)
        {
            elementBeforeEdit = new()
            {
                DataPanelSpecificationId = ((DataPanelSpecification)element).DataPanelSpecificationId,
                DataSpecification = ((DataPanelSpecification)element).DataSpecification,
                ItemOrder = ((DataPanelSpecification)element).ItemOrder,
                IsActive = ((DataPanelSpecification)element).IsActive
            };

        }

        private void ResetItemToOriginalValues(object element)
        {
            ((DataPanelSpecification)element).DataPanelSpecificationId = elementBeforeEdit.DataPanelSpecificationId;
            ((DataPanelSpecification)element).DataSpecification = elementBeforeEdit.DataSpecification;
            ((DataPanelSpecification)element).ItemOrder = elementBeforeEdit.ItemOrder;
            ((DataPanelSpecification)element).IsActive = elementBeforeEdit.IsActive;
        }

        private void ItemHasBeenCommitted(object element)
        {
            Console.WriteLine("Comitt item");
        }

    }
}