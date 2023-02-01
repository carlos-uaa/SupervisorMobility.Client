using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.PlantPage.AreaPage
{
    public partial class CreateArea
    {
        // Parameters
        [Parameter]
        public int PlantId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Plants", href: "/plants"),
            new BreadcrumbItem("PlantDetail", href: ""),
            new BreadcrumbItem("New Area", href: "", disabled: true)
        };

        // Objects
        Plant _plant = new();
        Area _area = new();

        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            _plant = await PlantService.GetPlantById(PlantId);
        }

        // Cancel form submit
        void CancelCreateOrUpdate()
        {
            NavigationManager.NavigateTo($"plants/{PlantId}");
        }

        // Create area
        async void CreateAreaAsync()
        {
            var result = await AreaService.CreateArea(PlantId, _area);
            NavigationManager.NavigateTo($"plants/{PlantId}");
        }
    }
}
