namespace SupervisorMobility.Client.Pages
{
    public partial class UpdatePlant
    {
        [Parameter]
        public int PlantId { get; set; }

        public Plant _plant { get; set; } = new();

        protected override async Task OnParametersSetAsync()
        {
            Plant dbPlant = await PlantService.GetPlantById(PlantId);
            _plant = dbPlant;
        }

        void UpdatePlantAsync()
        {
            PlantService.UpdatePlant(_plant);
            NavigationManager.NavigateTo($"plants");
        }
    }
}
