using Microsoft.JSInterop;
using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class DistributionDetail
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

        private List<Operation> _operations = new();

        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Plants", href: "/plants"),
            new BreadcrumbItem("PlantDetail", href: ""),
            new BreadcrumbItem("AreaDetail", href: ""),
            new BreadcrumbItem("DistributionDetail", href: "", disabled: true)
        };

        protected override async Task OnParametersSetAsync()
        {
            _distribution = await DistributionService.GetDistributionById(PlantId, AreaId, DistributionId);
            _area = await AreaService.GetAreaIncludingOperations(PlantId, AreaId);
            _plant = await PlantService.GetPlantById(PlantId);
            _operations = await OperationService.GetOperations(PlantId, AreaId, DistributionId);
        }

        void GoToPlant()
        {
            NavigationManager.NavigateTo($"plants/{PlantId}");
        }

        void GoToArea()
        {
            NavigationManager.NavigateTo($"plants/{PlantId}/areas/{AreaId}");
        }

        void CreateOperation()
        {
            NavigationManager.NavigateTo($"plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}/createoperation");
        }

        void EditOperation(int operationId)
        {
            NavigationManager.NavigateTo($"plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}/operations/updateoperation/{operationId}");
        }

        async Task DeleteOperation(int operationId)
        {
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this operation?");

            if (confirm)
            {
                _operations.RemoveAll(operation => operation.OperationId == operationId);
                await OperationService.DeleteOperation(PlantId, AreaId, DistributionId, operationId);
            }
        }
    }
}
