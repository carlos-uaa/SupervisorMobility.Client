using Microsoft.JSInterop;
using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class Plants
    {
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Plants", href: "", disabled: true)
        };

        public List<Plant> _plants { get; set; } = new();
        Plant _plant = new();

        protected async override Task OnInitializedAsync()
        {
            _plants = await PlantService.GetPlants();
        }

        void EditPlant(int plantId)
        {
            NavigationManager.NavigateTo($"plants/updateplant/{plantId}");
        }

        void CreatePlant()
        {
            NavigationManager.NavigateTo($"plants/createplant");
        }
        void PlantDetails(int plantId)
        {
            NavigationManager.NavigateTo($"plants/{plantId}");
        }

        async Task DeletePlant(int plantId)
        {
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this plant?");

            if (confirm)
            {
                _plants.RemoveAll(plant => plant.PlantId == plantId);
                await PlantService.DeletePlant(plantId);
            }
        }
    }
}
