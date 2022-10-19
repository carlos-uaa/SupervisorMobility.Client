using Microsoft.JSInterop;

namespace SupervisorMobility.Client.Shared
{
    public partial class DeleteOperationButton
    {
        [CascadingParameter(Name = "PlantId")]
        public int PlantId { get; set; }

        [CascadingParameter(Name = "AreaId")]
        public int AreaId { get; set; }

        [CascadingParameter(Name = "OperationId")]
        public int OperationId { get; set; }

        async void DeleteOperation()
        {
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this operation?");

            if (confirm)
            {
                await OperationService.DeleteOperation(PlantId, AreaId, OperationId);
                NavigationManager.NavigateTo($"/plants/plant/{PlantId}/areas/area/{AreaId}", forceLoad: true);
            }
        }
    }
}
