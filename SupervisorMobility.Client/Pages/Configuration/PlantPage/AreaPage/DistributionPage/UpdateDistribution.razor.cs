using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.PlantPage.AreaPage.DistributionPage
{
    public partial class UpdateDistribution
    {
        // Parameters
        [Parameter]
        public int PlantId { get; set; }

        [Parameter]
        public int AreaId { get; set; }

        [Parameter]
        public int DistributionId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links;

        // Objects
        Plant _plant = new();
        Area _area = new();
        public Distribution _distribution { get; set; } = new();

        // Initialization
        //protected async override Task OnInitializedAsync()
        //{
        //    _links = new List<BreadcrumbItem>
        //    {
        //        new BreadcrumbItem(text: Localizer["home"], href: "/"),
        //        new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
        //        new BreadcrumbItem(text: Localizer["plants"], href: "/plants"),
        //        new BreadcrumbItem(text: Localizer["plantDetails"], href: ""),
        //        new BreadcrumbItem(text: Localizer["areaDetails"], href: ""),
        //        new BreadcrumbItem(text: Localizer["updateDistribution"], href: "", disabled: true)
        //    };
        //    BreadcrumbService.UpdateBreadcrumbs(_links);
        //}

        protected override async Task OnParametersSetAsync()
        {
            _plant = await PlantService.GetPlantById(PlantId);
            _area = await AreaService.GetAreaById(PlantId, AreaId);
            _distribution = await DistributionService.GetDistributionById(PlantId, AreaId, DistributionId);
            _links = new List<BreadcrumbItem>
                 {
                        new BreadcrumbItem(text: Localizer["home"], href: "/"),
                        new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                        new BreadcrumbItem(text: Localizer["plants"], href: "/plants"),
                        new BreadcrumbItem(text: _plant.Code, href: $"plants/{PlantId}"),
                        new BreadcrumbItem(text: _area.Code, href: $"plants/{PlantId}/areas/{AreaId}"),
                        new BreadcrumbItem(text: _distribution.Description, href: $"plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}"),
                        new BreadcrumbItem(text: Localizer["updateDistribution"], href: "", disabled: true)
                 };
            BreadcrumbService.UpdateBreadcrumbs(_links);

           
        }

        // Links
        void GoToPlant()
        {
            NavigationManager.NavigateTo($"plants/{PlantId}");
        }

        void GoToArea()
        {
            NavigationManager.NavigateTo($"plants/{PlantId}/areas/{AreaId}");
        }

        // Update distribution
        async void UpdateDistributionAsync()
        {
            _distribution.IsActive = true;
            var result = await DistributionService.UpdateDistribution(PlantId, AreaId, _distribution);

            if (result)
            {
                NavigationManager.NavigateTo($"/plants/{PlantId}/areas/{AreaId}");
            }

        }

        // Cancel submit form
        void CancelCreateOrUpdate()
        {
            NavigationManager.NavigateTo($"/plants/{PlantId}/areas/{AreaId}");
        }
    }
}
