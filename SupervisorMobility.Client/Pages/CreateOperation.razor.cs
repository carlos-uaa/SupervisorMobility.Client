namespace SupervisorMobility.Client.Pages
{
    public partial class CreateOperation
    {
        [Parameter]
        public int PlantId { get; set; }

        [Parameter]
        public int AreaId { get; set; }

        Operation _operation = new();

        async void CreateOperationAsync()
        {
            var result = await OperationService.CreateOperation(PlantId, AreaId, _operation);
            NavigationManager.NavigateTo($"/plants/plant/{PlantId}/areas/area/{AreaId}");
        }

        void CancelCreateOrUpdate()
        {
            NavigationManager.NavigateTo($"/plants/plant/{PlantId}/areas/area/{AreaId}");
        }
    }
}
