namespace SupervisorMobility.Client.Pages
{
    public partial class CreateArea
    {
        [Parameter]
        public int PlantId { get; set; }

        Area _area = new();

        async void CreateAreaAsync()
        {
            var result = await AreaService.CreateArea(PlantId, _area);
            NavigationManager.NavigateTo($"plants/plant/{PlantId}");
        }

        void CancelCreateOrUpdate()
        {
            NavigationManager.NavigateTo($"plants/plant/{PlantId}");
        }
    }
}
