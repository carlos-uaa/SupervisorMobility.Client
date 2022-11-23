using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class CreateOperation
    {
        [Parameter]
        public int PlantId { get; set; }

        [Parameter]
        public int AreaId { get; set; }

        [Parameter]
        public int DistributionId { get; set; }

        Plant _plant = new();
        Area _area = new();
        Distribution _distribution = new();
        Operation _operation = new();

        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Plants", href: "/plants"),
            new BreadcrumbItem("PlantDetail", href: ""),
            new BreadcrumbItem("AreaDetail", href: ""),
            new BreadcrumbItem("DistributionDetail", href: ""),
            new BreadcrumbItem("New Operation", href: "", disabled: true)
        };

        async void CreateOperationAsync()
        {
            var result = await OperationService.CreateOperation(PlantId, AreaId, DistributionId, _operation);
            NavigationManager.NavigateTo($"/plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}");
        }

        void CancelCreateOrUpdate()
        {
            NavigationManager.NavigateTo($"/plants/{PlantId}/areas/{AreaId}");
        }

        protected override async Task OnParametersSetAsync()
        {
            _plant = await PlantService.GetPlantById(PlantId);
            _area = await AreaService.GetAreaById(PlantId, AreaId);
            _distribution = await DistributionService.GetDistributionById(PlantId, AreaId, DistributionId);
        }

        void GoToPlant()
        {
            NavigationManager.NavigateTo($"plants/{PlantId}");
        }

        void GoToArea()
        {
            NavigationManager.NavigateTo($"plants/{PlantId}/areas/{AreaId}");
        }

        void GoToDistribution()
        {
            NavigationManager.NavigateTo($"plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}");
        }
    }
}
