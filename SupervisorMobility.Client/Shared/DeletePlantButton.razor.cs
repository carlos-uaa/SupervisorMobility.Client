using Microsoft.JSInterop;

namespace SupervisorMobility.Client.Shared
{
    public partial class DeletePlantButton
    {
        [CascadingParameter]
        public int PlantId { get; set; }

        async void DeletePlant()
        {
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this plant?");

            if (confirm)
            {
                await PlantService.DeletePlant(PlantId);
                NavigationManager.NavigateTo($"plants");
            }
        }
    }
}
