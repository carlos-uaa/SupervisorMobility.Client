using MudBlazor;
using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Pages.Configuration.PlantPage.AreaPage.DistributionPage
{
    public partial class CreateDistribution
    {
        // Parameters
        [Parameter]
        public int PlantId { get; set; }

        [Parameter]
        public int AreaId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links;

        // Objects
        Plant _plant = new();
        Area _area = new();
        Distribution _distribution = new();

        //protected async override Task OnInitializedAsync()
        //{
        //    _links = new List<BreadcrumbItem>
        //    {
        //        new BreadcrumbItem(text: Localizer["home"], href: "/"),
        //        new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
        //        new BreadcrumbItem(text: Localizer["plants"], href: "/plants"),
        //        new BreadcrumbItem(text: Localizer["plantDetails"], href: ""),
        //        new BreadcrumbItem(text: Localizer["areaDetails"], href: ""),
        //        new BreadcrumbItem(text: Localizer["newDistribution"], href: "", disabled: true)
        //    };
        //    BreadcrumbService.UpdateBreadcrumbs(_links);
        //}

        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            _plant = await PlantService.GetPlantById(PlantId);
            _area = await AreaService.GetAreaById(PlantId, AreaId);
            _links = new List<BreadcrumbItem>
                 {
                     new BreadcrumbItem(text: Localizer["home"], href: "/"),
                     new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                     new BreadcrumbItem(text: Localizer["plants"], href: "/plants"),
                     new BreadcrumbItem(text: _plant.Code, href: $"plants/{PlantId}"),
                     new BreadcrumbItem(text: _area.Code, href: $"plants/{PlantId}/areas/{AreaId}"),
                     new BreadcrumbItem(text: Localizer["newDistribution"], href: "", disabled: true)
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

        // Cancel submit form
        void CancelCreateOrUpdate()
        {
            NavigationManager.NavigateTo($"/plants/{PlantId}/areas/{AreaId}");
        }

        // Create distribution
        async void CreateDistributionAsync()
        {
            _distribution.IsActive = true;
            var result = await DistributionService.CreateDistribution(PlantId, AreaId, _distribution);
            NavigationManager.NavigateTo($"/plants/{PlantId}/areas/{AreaId}");
        }
    }
}
