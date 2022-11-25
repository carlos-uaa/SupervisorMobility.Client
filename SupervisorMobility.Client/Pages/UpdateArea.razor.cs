using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class UpdateArea
    {
        // Parameters
        [Parameter]
        public int plantId { get; set; }

        [Parameter]
        public int areaId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Plants", href: "/plants"),
            new BreadcrumbItem("PlantDetail", href: ""),
            new BreadcrumbItem("UpdateArea", href: "", disabled: true)
        };

        // Objects
        Plant _plant = new();
        public Area _area { get; set; } = new();

        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            Area dbArea = await AreaService.GetAreaById(plantId, areaId);
            _plant = await PlantService.GetPlantById(plantId);
            _area = dbArea;
        }

        // Cancel submit form
        void CancelCreateOrUpdate()
        {
            NavigationManager.NavigateTo($"plants/{plantId}");
        }

        // Update area
        void UpdateAreaAsync()
        {
            AreaService.UpdateArea(plantId, _area);
            NavigationManager.NavigateTo($"plants/{plantId}");
        }
    }
}
