using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.PlantPage
{
    public partial class CreatePlant
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links;

        Plant _plant = new();
        protected async override Task OnInitializedAsync()
        {
            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                new BreadcrumbItem(text: Localizer["plants"], href: "/plants"),
                new BreadcrumbItem(text: Localizer["newPlant"], href: "", disabled: true)
            };
            BreadcrumbService.UpdateBreadcrumbs(_links);

        }


        // Create plant
        async void CreatePlantAsync()
        {
            _plant.IsActive = true;
            var result = await PlantService.CreatePlant(_plant);
            NavigationManager.NavigateTo($"plants");
        }
    }
}
