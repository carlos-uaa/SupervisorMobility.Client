namespace SupervisorMobility.Client.Pages
{
    public partial class Plants
    {
        public List<Plant> _plants { get; set; } = new();
        Plant _plant = new();

        protected async override Task OnInitializedAsync()
        {
            _plants = await PlantService.GetPlants();
        }

        void EditPlant(int plantId)
        {
            NavigationManager.NavigateTo($"plants/plant/updateplant/{plantId}");
        }

        void CreatePlant()
        {
            NavigationManager.NavigateTo($"plants/plant");
        }
        void PlantDetails(int plantId)
        {
            NavigationManager.NavigateTo($"plants/plant/{plantId}");
        }
    }
}
