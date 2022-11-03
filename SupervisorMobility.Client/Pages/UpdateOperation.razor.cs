using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class UpdateOperation
    {
        [Parameter]
        public int PlantId { get; set; }

        [Parameter]
        public int AreaId { get; set; }

        [Parameter]
        public int OperationId { get; set; }

        Plant _plant = new();
        Area _area = new();
        public Operation _operation { get; set; } = new();

        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Plants", href: "/plants"),
            new BreadcrumbItem("PlantDetail", href: ""),
            new BreadcrumbItem("AreaDetail", href: ""),
            new BreadcrumbItem("UpdateOperation", href: "", disabled: true)
        };

        protected override async Task OnParametersSetAsync()
        {
            Operation dbOperation = await OperationService.GetOperationById(PlantId, AreaId, OperationId);
            _plant = await PlantService.GetPlantById(PlantId);
            _area = await AreaService.GetAreaById(PlantId, AreaId);
            _operation = dbOperation;
        }

        void UpdateOperationAsync()
        {
            OperationService.UpdateOperation(PlantId, AreaId, _operation);
            NavigationManager.NavigateTo($"/plants/{PlantId}/areas/{AreaId}");
        }

        void CancelCreateOrUpdate()
        {
            NavigationManager.NavigateTo($"/plants/{PlantId}/areas/{AreaId}");
        }

        void GoToPlant()
        {
            NavigationManager.NavigateTo($"plants/{PlantId}");
        }
    }
}
