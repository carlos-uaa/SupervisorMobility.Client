using Microsoft.JSInterop;

namespace SupervisorMobility.Client.Shared
{
    public partial class AreaDistributions
    {
        [CascadingParameter(Name = "PlantId")]
        public int plantId { get; set; }

        [CascadingParameter(Name = "AreaId")]
        public int areaId { get; set; }

        public List<Distribution> _distributions { get; set; } = new();

        protected async override Task OnInitializedAsync()
        {
            _distributions = await DistributionService.GetDistributions(plantId, areaId);
        }

        void CreateDistribution()
        {
            NavigationManager.NavigateTo($"plants/{plantId}/areas/{areaId}/distributions/createdistribution");
        }

        void EditDistribution(int distributionId)
        {
            NavigationManager.NavigateTo($"plants/{plantId}/areas/{areaId}/updatedistribution/{distributionId}");
        }

        async Task DeleteDistribution(int distributionId)
        {
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this distribution?");

            if (confirm)
            {
                _distributions.RemoveAll(distribution => distribution.DistributionId == distributionId);
                await DistributionService.DeleteDistribution(plantId, areaId, distributionId);
            }
        }

        void DistributionDetails(int distributionId)
        {
            NavigationManager.NavigateTo($"plants/{plantId}/areas/{areaId}/distributions/{distributionId}");
        }
    }
}
