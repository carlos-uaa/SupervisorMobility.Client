using Microsoft.JSInterop;
using MudBlazor;

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
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this plant?");

            if (confirm)
            {
                _plants.RemoveAll(plant => plant.PlantId == plantId);
                await PlantService.DeletePlant(plantId);
            }
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
    }
}
