namespace SupervisorMobility.Client.Pages
{
    public partial class UpdateArea
    {
        [Parameter]
        public int plantId { get; set; }

        [Parameter]
        public int areaId { get; set; }

        public Area _area { get; set; } = new();

        protected override async Task OnParametersSetAsync()
        {
            Area dbArea = await AreaService.GetAreaById(plantId, areaId);
            _area = dbArea;
        }

        void UpdateAreaAsync()
        {
            AreaService.UpdateArea(plantId, _area);
            NavigationManager.NavigateTo($"plants/plant/{plantId}");
        }
    }
}
