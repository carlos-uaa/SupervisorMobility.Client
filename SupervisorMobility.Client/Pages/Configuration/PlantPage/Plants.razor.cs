using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Pages.Components;
using static MudBlazor.CategoryTypes;

namespace SupervisorMobility.Client.Pages.Configuration.PlantPage
{
    public partial class Plants
    {

        //Breadcrumb links
        private List<BreadcrumbItem> _links;
        //variables to table
        private bool dense = false;
        private bool hover = true;
        private bool striped = false;
        private bool bordered = false;
        private string searchString1 = "";

        // Objects
        public List<Plant> _plants { get; set; } = new();
        Plant _plant = new();

        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };

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
                new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                new BreadcrumbItem(text: Localizer["plants"], href: "", disabled: true)
            };
            BreadcrumbService.UpdateBreadcrumbs(_links);
            _plants = await PlantService.GetPlants();
        }

        // Create plant
        void CreatePlant()
        {
            NavigationManager.NavigateTo($"plants/createplant");
        }

        // Delete plant
        async Task DeletePlant(int plantId)
        {
            _plants.RemoveAll(plant => plant.PlantId == plantId);
            await PlantService.DeletePlant(plantId);

            visibleDelete = false;
        }

        // Plant details
        void PlantDetails(int plantId)
        {
            NavigationManager.NavigateTo($"plants/{plantId}");
        }

        // Update plant
        void UpdatePlant(int plantId)
        {
            NavigationManager.NavigateTo($"plants/updateplant/{plantId}");
        }

        //filter function
        private bool FilterFunc1(Plant element) => FilterFunc(element, searchString1);
        private bool FilterFunc(Plant element, string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.PlantId.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Code.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{element.PlantId} {element.Code} {element.Description}".Contains(searchString))
                return true;
            return false;
        }


        //Delete Plant 
        private bool visibleDelete = false;
        public int deletePlantId = 0;
        private void OpenDeleteDialog(int deleteId)
        {
            deletePlantId = deleteId;
            visibleDelete = true;
        }
        void CloseDeleteModal() => visibleDelete = false;
        private DialogOptions dialogDeleteOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, Position = DialogPosition.TopCenter, DisableBackdropClick = true, CloseButton = true };


        private int selectedRowNumber = -1;
        private MudTable<Plant> SelectTableEvent;

        private void RowClickEvent(TableRowClickEventArgs<Plant> args)
        {
           
            var visibleItems = SelectTableEvent.FilteredItems.ToList();
            var rowIndex = visibleItems.IndexOf(args.Item);

            if (selectedRowNumber == rowIndex)
            {
                NavigationManager.NavigateTo($"plants/{args.Item.PlantId}");
            }
            else
            {
                SelectTableEvent.SelectedItem = args.Item;
                selectedRowNumber = rowIndex;
                StateHasChanged();  
            }
        }

        private string SelectedRowClassFunc(Plant element, int rowNumber)
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
