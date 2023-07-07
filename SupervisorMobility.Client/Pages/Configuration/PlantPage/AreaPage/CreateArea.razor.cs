using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.PlantPage.AreaPage
{
    public partial class CreateArea
    {
        // Parameters
        [Parameter]
        public int PlantId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links;

        // Objects
        Plant _plant = new();
        Area _area = new();

        // Initialization
        protected async override Task OnInitializedAsync()
        {
            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "#"),
                new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                new BreadcrumbItem(text: Localizer["plants"], href: "/plants"),
                new BreadcrumbItem(text: Localizer["plantDetails"], href: ""),
                new BreadcrumbItem(text: Localizer["newArea"], href: "", disabled: true)
            };
        }

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
            _area.IsActive = true;
            var result = await AreaService.CreateArea(PlantId, _area);
            NavigationManager.NavigateTo($"plants/{PlantId}");
        }
    }
}
