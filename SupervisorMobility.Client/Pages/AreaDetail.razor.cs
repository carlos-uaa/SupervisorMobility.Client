namespace SupervisorMobility.Client.Pages
{
    public partial class AreaDetail
    {
        [Parameter]
        public int PlantId { get; set; }

        [Parameter]
        public int AreaId { get; set; }

        Area _area = new();

        protected override async Task OnParametersSetAsync()
        {
            _area = await AreaService.GetAreaIncludingOperations(PlantId, AreaId);
        }
    }
}
