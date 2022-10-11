namespace SupervisorMobility.Client.Pages
{
    public partial class CreateArea
    {
        [Parameter]
        public int Id { get; set; }

        Area _area = new();

        async void CreateAreaAsync()
        {
            var result = await AreaService.CreateArea(Id, _area);
            NavigationManager.NavigateTo($"plants/plant/{Id}");
        }

        void CancelCreateOrUpdate()
        {
            NavigationManager.NavigateTo($"plants/plant/{Id}");
        }
    }
}
