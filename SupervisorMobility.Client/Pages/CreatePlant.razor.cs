using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class CreatePlant
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Plants", href: "/plants"),
            new BreadcrumbItem("New plant", href: "", disabled: true)
        };

        // Objects
        Plant _plant = new();

        // Create plant
        async void CreatePlantAsync()
        {
            var result = await PlantService.CreatePlant(_plant);
            NavigationManager.NavigateTo($"plants");
        }
    }
}
