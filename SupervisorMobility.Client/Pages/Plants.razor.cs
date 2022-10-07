namespace SupervisorMobility.Client.Pages
{
    public partial class Plants
    {
        public List<Plant> _plants { get; set; } = new();

        protected async override Task OnInitializedAsync()
        {
            _plants = await PlantService.GetPlants();
        }
    }
}
