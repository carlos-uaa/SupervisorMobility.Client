using MudBlazor;
using SupervisorMobility.Client.Services.BreadcrumsService;

namespace SupervisorMobility.Client.Pages.Configuration.PlantPage
{
    public partial class UpdatePlant
    {
        // Parameters
        [Parameter]
        public int PlantId { get; set; }
       
        // Breadcrumb links
        private List<BreadcrumbItem> _links;

        // Objects
        public Plant _plant { get; set; } = new();

        // Initialization
        protected async override Task OnInitializedAsync()
        {
            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                new BreadcrumbItem(text: Localizer["plants"], href: "/plants"),
                new BreadcrumbItem(text: Localizer["updatePlant"], href: "", disabled: true)
            };
            BreadcrumbService.UpdateBreadcrumbs(_links);
        }


        protected override async Task OnParametersSetAsync()
        {
            Plant dbPlant = await PlantService.GetPlantById(PlantId);
            _plant = dbPlant;
        }

        // Update plant
        async void UpdatePlantAsync()
        {
            _plant.IsActive = true;
            var result = await PlantService.UpdatePlant(_plant);

            if (result)
            {
                NavigationManager.NavigateTo($"plants");
            }

        }
    }
}
