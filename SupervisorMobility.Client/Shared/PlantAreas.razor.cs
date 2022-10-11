namespace SupervisorMobility.Client.Shared
{
    public partial class PlantAreas
    {
        [CascadingParameter]
        public int Id { get; set; }

        public Plant _plant { get; set; } = new();

        protected async override Task OnInitializedAsync()
        {
            _plant = await PlantService.GetPlantIncludingAreas(Id);
        }

        void CreateArea(int PlantId)
        {
            NavigationManager.NavigateTo($"plants/plant/{PlantId}/createarea");
        }
    }
}
