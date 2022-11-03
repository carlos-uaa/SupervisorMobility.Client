using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class CreateArea
    {
        [Parameter]
        public int PlantId { get; set; }

        Plant _plant = new();
        Area _area = new();

        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Plants", href: "/plants"),
            new BreadcrumbItem("PlantDetail", href: ""),
            new BreadcrumbItem("New Area", href: "", disabled: true)
        };

        async void CreateAreaAsync()
        {
            var result = await AreaService.CreateArea(PlantId, _area);
            NavigationManager.NavigateTo($"plants/{PlantId}");
        }

        void CancelCreateOrUpdate()
        {
            NavigationManager.NavigateTo($"plants/{PlantId}");
        }

        protected override async Task OnParametersSetAsync()
        {
            _plant = await PlantService.GetPlantById(PlantId);
        }
    }
}
