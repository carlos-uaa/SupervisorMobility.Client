using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.IS;

namespace SupervisorMobility.Client.Pages.IS.ConfigurationIS.DataPanelPage
{
    public partial class DataPanelCategoryIndex
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>();

        // Objects
        public List<DataPanel> _dataPanelsCategories { get; set; } = new();

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
                new BreadcrumbItem(text: Localizer["DataPanel"], href: "", disabled: true)
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

                _dataPanelsCategories = await DataPanelServices.GetAllDataPanels();

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


        DialogOptions options = new DialogOptions
        {
            MaxWidth = MaxWidth.Medium,
            FullWidth = true,
            CloseButton = true,
            DisableBackdropClick = true // Evita que se cierre por error al hacer clic fuera
        };

        // Create CreateDataPanel
        async void CreateDataPanel()
        {
            var parameters = new DialogParameters
            {
                ["Type"] = "Create",
                ["DataPanelId"] = 0
            };

            var dialog = await DialogService.ShowAsync<DataPanelForm>("New DataPanel", parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
                StateHasChanged();
        }

        // Details CreateDataPanel
        async void DataPanelDetails(int datapanelId)
        {
            var parameters = new DialogParameters
            {
                ["Type"] = "Details",
                ["DataPanelId"] = datapanelId
            };

            var dialog = await DialogService.ShowAsync<DataPanelForm>("DataPanel", parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
                StateHasChanged();
        }

        // Reorder CreateDataPanel
        void ReOrderDataPanel()
        {
            NavigationManager.NavigateTo($"configurationIS/DataPanels/sequence");
        }
        // Update category
        async void UpdateCategory(int categoryId)
        {
            var parameters = new DialogParameters
            {
                ["Type"] = "Update",
                ["DataPanelId"] = categoryId // ID en 0 para nuevo registro
            };

            var dialog = await DialogService.ShowAsync<DataPanelForm>("Edit DataPanel", parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
                StateHasChanged();
        }

        async Task DeleteDataPanel(int datapanelId)
        {
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this DataPanel?");

            if (confirm)
            {
                _dataPanelsCategories.RemoveAll(category => category.DataPanelId == datapanelId);
                await DataPanelServices.DeleteDataPanel(datapanelId);
                StateHasChanged();
            }
        }

        private string searchString = "";

        private bool FilterFunc(DataPanel element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;



            if (element.DataTitle.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;

            var searchWords = searchString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Verificar si todas las palabras en searchWords están contenidas en DataTitle
            if (searchWords.All(word => element.DataTitle.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0))
                return true;


            return false;
        }

        private int selectedRowNumber = -1;
        private MudTable<DataPanel> SelectTableEvent;

        private void RowClickEvent(TableRowClickEventArgs<DataPanel> args)
        {
            var visibleItems = SelectTableEvent.FilteredItems.ToList();
            var rowIndex = visibleItems.IndexOf(args.Item);

            if (selectedRowNumber == rowIndex)
            {
                    DataPanelDetails(args.Item.DataPanelId);
               
            }
            else
            {
                SelectTableEvent.SelectedItem = args.Item;
                selectedRowNumber = rowIndex;
                StateHasChanged();
            }
        }

        private string SelectedRowClassFunc(DataPanel element, int rowNumber)
        {
            var visibleItems = SelectTableEvent.FilteredItems.ToList();

            if (selectedRowNumber == rowNumber)
            {
                return "selected"; // Marca la fila seleccionada
            }
            else if (SelectTableEvent.SelectedItem != null && SelectTableEvent.SelectedItem.Equals(element))
            {
                selectedRowNumber = visibleItems.IndexOf(element);  // Usa el índice filtrado
                return "selected";
            }
            else
            {
                return string.Empty;
            }
        }



    }
}
