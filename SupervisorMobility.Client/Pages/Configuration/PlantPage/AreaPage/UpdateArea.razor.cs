using DocumentFormat.OpenXml.Vml.Spreadsheet;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Services.BreadcrumsService;

namespace SupervisorMobility.Client.Pages.Configuration.PlantPage.AreaPage
{
    public partial class UpdateArea
    {
        // Parameters
        [Parameter]
        public int plantId { get; set; }

        [Parameter]
        public int areaId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links;
        [Inject]
        private IBreadcrumbService BreadcrumbService { get; set; }
        // Objects
        Plant _plant = new();
        public Area _area { get; set; } = new();

        // Initialization
        //protected async override Task OnInitializedAsync()
        //{
        //    _links = new List<BreadcrumbItem>
        //    {
        //        new BreadcrumbItem(text: Localizer["home"], href: "/"),
        //        new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
        //        new BreadcrumbItem(text: Localizer["plants"], href: "/plants"),
        //        new BreadcrumbItem(text: Localizer["plantDetails"], href: ""),
        //        new BreadcrumbItem(text: Localizer["update"] + " " + Localizer["area"], href: "", disabled: true)
        //    };
        //    BreadcrumbService.UpdateBreadcrumbs(_links);
        //}

        protected override async Task OnParametersSetAsync()
        {
            Area dbArea = await AreaService.GetAreaById(plantId, areaId);
            _plant = await PlantService.GetPlantById(plantId);
            _area = dbArea;


                    _links = new List<BreadcrumbItem>
             {
                 new BreadcrumbItem(text: Localizer["home"], href: "/"),
                 new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                 new BreadcrumbItem(text: Localizer["plants"], href: "/plants"),
                 new BreadcrumbItem(text: _plant.Code, href: $"plants/{plantId}"),
                 new BreadcrumbItem(text: _area.Code, href: $"plants/{plantId}/areas/{areaId}"),
                new BreadcrumbItem(text: Localizer["update"] + " " + Localizer["area"], href: "", disabled: true)

             };
             BreadcrumbService.UpdateBreadcrumbs(_links);
        }

        // Cancel submit form
        void CancelCreateOrUpdate()
        {
            NavigationManager.NavigateTo($"plants/{plantId}");
        }

        // Update area
        async void UpdateAreaAsync()
        {
            _area.IsActive = true;
            var result = await AreaService.UpdateArea(plantId, _area);

            if (result)
            {
                NavigationManager.NavigateTo($"plants/{plantId}");
            }

        }
    }
}
