using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class AreaDetail
    {
        [Parameter]
        public int PlantId { get; set; }

        [Parameter]
        public int AreaId { get; set; }

        Plant _plant = new();
        Area _area = new();

        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Plants", href: "/plants"),
            new BreadcrumbItem("PlantDetail", href: ""),
            new BreadcrumbItem("AreaDetail", href: "", disabled: true)
        };

        protected override async Task OnParametersSetAsync()
        {
            _area = await AreaService.GetAreaIncludingOperations(PlantId, AreaId);
            _plant = await PlantService.GetPlantById(PlantId);
        }

        void GoToPlant()
        {
            NavigationManager.NavigateTo($"plants/{PlantId}");
        }
    }
}
