using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class PlantDetail
    {
        [Parameter]
        public int PlantId { get; set; }

        Plant _plant = new();

        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Plants", href: "/plants"),
            new BreadcrumbItem("PlantDetail", href: "", disabled: true)
        };

        protected override async Task OnParametersSetAsync()
        {
            _plant = await PlantService.GetPlantById(PlantId);
        }
    }
}
