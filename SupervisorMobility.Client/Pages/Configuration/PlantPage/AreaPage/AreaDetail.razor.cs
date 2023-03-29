using Microsoft.JSInterop;
using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.PlantPage.AreaPage
{
    public partial class AreaDetail
    {
        // Parameters
        [Parameter]
        public int PlantId { get; set; }

        [Parameter]
        public int AreaId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Plants", href: "/plants"),
            new BreadcrumbItem("PlantDetail", href: ""),
            new BreadcrumbItem("AreaDetail", href: "", disabled: true)
        };

        // Objects
        Plant _plant = new();
        Area _area = new();
        public List<Distribution> _distributions { get; set; } = new();

        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            _area = await AreaService.GetOneAreaIncludingCollections(PlantId, AreaId);
            _plant = await PlantService.GetPlantById(PlantId);
            _distributions = await DistributionService.GetDistributions(PlantId, AreaId);
        }

        // Links
        void GoToPlant()
        {
            NavigationManager.NavigateTo($"plants/{PlantId}");
        }

        // Create distribution
        void CreateDistribution()
        {
            NavigationManager.NavigateTo($"plants/{PlantId}/areas/{AreaId}/distributions/createdistribution");
        }

        // Delete distribution
        async Task DeleteDistribution(int distributionId)
        {
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this distribution?");

            if (confirm)
            {
                _distributions.RemoveAll(distribution => distribution.DistributionId == distributionId);
                await DistributionService.DeleteDistribution(PlantId, AreaId, distributionId);
            }
        }

        // Distribution details
        void DistributionDetails(int distributionId)
        {
            NavigationManager.NavigateTo($"plants/{PlantId}/areas/{AreaId}/distributions/{distributionId}");
        }

        // Update distribution
        void UpdateDistribution(int distributionId)
        {
            NavigationManager.NavigateTo($"plants/{PlantId}/areas/{AreaId}/updatedistribution/{distributionId}");
        }
    }
}
