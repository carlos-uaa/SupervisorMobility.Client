using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class CreateDistribution
    {
        [Parameter]
        public int PlantId { get; set; }

        [Parameter]
        public int AreaId { get; set; }

        Plant _plant = new();
        Area _area = new();
        Distribution _distribution = new();

        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Plants", href: "/plants"),
            new BreadcrumbItem("PlantDetail", href: ""),
            new BreadcrumbItem("AreaDetail", href: ""),
            new BreadcrumbItem("New Distribution", href: "", disabled: true)
        };

        async void CreateDistributionAsync()
        {
            var result = await DistributionService.CreateDistribution(PlantId, AreaId, _distribution);
            NavigationManager.NavigateTo($"/plants/{PlantId}/areas/{AreaId}");
        }

        void CancelCreateOrUpdate()
        {
            NavigationManager.NavigateTo($"/plants/{PlantId}/areas/{AreaId}");
        }

        protected override async Task OnParametersSetAsync()
        {
            _plant = await PlantService.GetPlantById(PlantId);
            _area = await AreaService.GetAreaById(PlantId, AreaId);
        }

        void GoToPlant()
        {
            NavigationManager.NavigateTo($"plants/{PlantId}");
        }

        void GoToArea()
        {
            NavigationManager.NavigateTo($"plants/{PlantId}/areas/{AreaId}");
        }
    }
}
