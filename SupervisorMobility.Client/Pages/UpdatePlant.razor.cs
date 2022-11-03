using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class UpdatePlant
    {
        [Parameter]
        public int PlantId { get; set; }

        public Plant _plant { get; set; } = new();

        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Plants", href: "/plants"),
            new BreadcrumbItem("UpdatePlant", href: "", disabled: true)
        };

        protected override async Task OnParametersSetAsync()
        {
            Plant dbPlant = await PlantService.GetPlantById(PlantId);
            _plant = dbPlant;
        }

        void UpdatePlantAsync()
        {
            PlantService.UpdatePlant(_plant);
            NavigationManager.NavigateTo($"plants");
        }
    }
}
