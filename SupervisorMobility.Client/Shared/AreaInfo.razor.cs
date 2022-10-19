namespace SupervisorMobility.Client.Shared
{
    public partial class AreaInfo
    {
        [CascadingParameter(Name = "PlantId")]
        public int plantId { get; set; }

        [CascadingParameter(Name = "AreaId")]
        public int areaId { get; set; }

        public Area _area { get; set; } = new();

        protected async override Task OnInitializedAsync()
        {
            _area = await AreaService.GetAreaIncludingOperations(plantId, areaId);
        }
    }
}
