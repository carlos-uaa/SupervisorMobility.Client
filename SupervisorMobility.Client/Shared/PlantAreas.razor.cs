namespace SupervisorMobility.Client.Shared
{
    public partial class PlantAreas
    {
        [CascadingParameter]
        public int PlantId { get; set; }

        public Plant _plant { get; set; } = new();

        protected async override Task OnInitializedAsync()
        {
            _plant = await PlantService.GetPlantIncludingAreas(PlantId);
        }

        void CreateArea(int PlantId)
        {
            NavigationManager.NavigateTo($"plants/plant/{PlantId}/createarea");
        }

        void UpdateArea(int plantId, int areaId)
        {
            NavigationManager.NavigateTo($"plants/plant/{plantId}/updatearea/{areaId}");
        }
        void AreaDetails(int plantId, int areaId)
        {
            NavigationManager.NavigateTo($"plants/plant/{plantId}/areas/area/{areaId}");
        }
    }
}
