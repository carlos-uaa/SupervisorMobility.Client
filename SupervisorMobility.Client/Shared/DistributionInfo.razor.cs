namespace SupervisorMobility.Client.Shared
{
    public partial class DistributionInfo
    {
        [CascadingParameter(Name = "PlantId")]
        public int PlantId { get; set; }

        [CascadingParameter(Name = "AreaId")]
        public int AreaId { get; set; }

        [CascadingParameter(Name = "DistributionId")]
        public int DistributionId { get; set; }

        public Distribution _distribution { get; set; } = new();

        protected async override Task OnInitializedAsync()
        {
            _distribution = await DistributionService.GetDistributionById(PlantId, AreaId, DistributionId);
        }
    }
}
