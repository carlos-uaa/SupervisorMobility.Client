using Microsoft.JSInterop;
using MudBlazor;
using static MudBlazor.CategoryTypes;

namespace SupervisorMobility.Client.Pages.Configuration.PlantPage
{
    public partial class Plants
    {

        //Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Plants", href: "", disabled: true)
        };



        //variables to table
        private bool dense = false;
        private bool hover = true;
        private bool striped = false;
        private bool bordered = false;
        private string searchString1 = "";

        // Objects
        public List<Plant> _plants { get; set; } = new();
        Plant _plant = new();

        // Initialization
        protected async override Task OnInitializedAsync()
        {
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
        private DialogOptions dialogDeleteOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, Position = DialogPosition.TopCenter };
    }
}
