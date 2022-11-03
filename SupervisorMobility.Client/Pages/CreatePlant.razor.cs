using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class CreatePlant
    {
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Plants", href: "/plants"),
            new BreadcrumbItem("New plant", href: "", disabled: true)
        };

        Plant _plant = new();

        async void CreatePlantAsync()
        {
            var result = await PlantService.CreatePlant(_plant);
            NavigationManager.NavigateTo($"plants");
        }
    }
}
