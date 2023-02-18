using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.PlantPage
{
    public partial class UpdatePlant
    {
        // Parameters
        [Parameter]
        public int PlantId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Plants", href: "/plants"),
            new BreadcrumbItem("UpdatePlant", href: "", disabled: true)
        };

        // Objects
        public Plant _plant { get; set; } = new();

        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            Plant dbPlant = await PlantService.GetPlantById(PlantId);
            _plant = dbPlant;
        }

        // Update plant
        async void UpdatePlantAsync()
        {
            var result = await PlantService.UpdatePlant(_plant);

            if (result)
            {
                NavigationManager.NavigateTo($"plants");
            }

        }
    }
}
