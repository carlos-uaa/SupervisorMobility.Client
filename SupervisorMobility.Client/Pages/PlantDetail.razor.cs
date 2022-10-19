namespace SupervisorMobility.Client.Pages
{
    public partial class PlantDetail
    {
        [Parameter]
        public int PlantId { get; set; }

        Plant _plant = new();

        protected override async Task OnParametersSetAsync()
        {
            _plant = await PlantService.GetPlantById(PlantId);
        }
    }
}
