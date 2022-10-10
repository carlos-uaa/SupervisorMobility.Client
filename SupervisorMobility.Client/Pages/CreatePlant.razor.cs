namespace SupervisorMobility.Client.Pages
{
    public partial class CreatePlant
    {
        Plant _plant = new();

        async void CreatePlantAsync()
        {
            var result = await PlantService.CreatePlant(_plant);
            NavigationManager.NavigateTo($"plants/plant/{result.PlantId}");
        }
    }
}
