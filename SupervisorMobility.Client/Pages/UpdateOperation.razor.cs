namespace SupervisorMobility.Client.Pages
{
    public partial class UpdateOperation
    {
        [Parameter]
        public int PlantId { get; set; }

        [Parameter]
        public int AreaId { get; set; }

        [Parameter]
        public int OperationId { get; set; }

        public Operation _operation { get; set; } = new();

        protected override async Task OnParametersSetAsync()
        {
            Operation dbOperation = await OperationService.GetOperationById(PlantId, AreaId, OperationId);
            _operation = dbOperation;
        }

        void UpdateOperationAsync()
        {
            OperationService.UpdateOperation(PlantId, AreaId, _operation);
            NavigationManager.NavigateTo($"/plants/plant/{PlantId}/areas/area/{AreaId}");
        }

        void CancelCreateOrUpdate()
        {
            NavigationManager.NavigateTo($"/plants/plant/{PlantId}/areas/area/{AreaId}");
        }
    }
}
