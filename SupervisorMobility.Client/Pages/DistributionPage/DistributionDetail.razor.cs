using Microsoft.JSInterop;
using MudBlazor;

namespace SupervisorMobility.Client.Pages.DistributionPage
{
    public partial class DistributionDetail
    {
        // Parameters
        [Parameter]
        public int PlantId { get; set; }

        [Parameter]
        public int AreaId { get; set; }

        [Parameter]
        public int DistributionId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Plants", href: "/plants"),
            new BreadcrumbItem("PlantDetail", href: ""),
            new BreadcrumbItem("AreaDetail", href: ""),
            new BreadcrumbItem("DistributionDetail", href: "", disabled: true)
        };

        // Objects
        Plant _plant = new();
        Area _area = new();
        Distribution _distribution = new();
        private List<Operation> _operations = new();

        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            _distribution = await DistributionService.GetDistributionById(PlantId, AreaId, DistributionId);
            _area = await AreaService.GetAreaIncludingOperations(PlantId, AreaId);
            _plant = await PlantService.GetPlantById(PlantId);
            _operations = await OperationService.GetOperations(PlantId, AreaId, DistributionId);
        }

        // Links
        void GoToPlant()
        {
            NavigationManager.NavigateTo($"plants/{PlantId}");
        }

        void GoToArea()
        {
            NavigationManager.NavigateTo($"plants/{PlantId}/areas/{AreaId}");
        }

        // Create operation
        void CreateOperation()
        {
            NavigationManager.NavigateTo($"plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}/createoperation");
        }

        // Delete operation
        async Task DeleteOperation(int operationId)
        {
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this operation?");

            if (confirm)
            {
                _operations.RemoveAll(operation => operation.OperationId == operationId);
                await OperationService.DeleteOperation(PlantId, AreaId, DistributionId, operationId);
            }
        }

        // Update operation
        void UpdateOperation(int operationId)
        {
            NavigationManager.NavigateTo($"plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}/operations/updateoperation/{operationId}");
        }


    }
}
