using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.PlantPage.AreaPage.DistributionPage.OperationPage
{
    public partial class DetailsOperation
    {
        // Parameters
        [Parameter]
        public int PlantId { get; set; }

        [Parameter]
        public int AreaId { get; set; }

        [Parameter]
        public int DistributionId { get; set; }

        [Parameter]
        public int OperationId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links;

        // Objects
        Plant _plant = new();
        Area _area = new();
        Distribution _distribution = new();
        public Operation _operation { get; set; } = new();
        private bool showui = false;

        // Initialization
        // protected async override Task OnInitializedAsync()
        // {
        //     _links = new List<BreadcrumbItem>
        //         {
        //             new BreadcrumbItem(text: Localizer["home"], href: "/"),
        //             new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
        //             new BreadcrumbItem(text: Localizer["plants"], href: "/plants"),
        //             new BreadcrumbItem(text: Localizer["plantDetails"], href: ""),
        //             new BreadcrumbItem(text: Localizer["areaDetails"], href: ""),
        //             new BreadcrumbItem(text: Localizer["distributionDetails"], href: ""),
        //             new BreadcrumbItem(text: Localizer["updateOperation"], href: "", disabled: true)
        //         };
        // }

        protected override async Task OnParametersSetAsync()
        {
            _plant = await PlantService.GetPlantById(PlantId);
            _area = await AreaService.GetAreaById(PlantId, AreaId);
            _distribution = await DistributionService.GetDistributionById(PlantId, AreaId, DistributionId);
            _operation = await OperationService.GetOperationById(PlantId, AreaId, DistributionId, OperationId);
            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                new BreadcrumbItem(text: Localizer["plants"], href: "/plants"),
                new BreadcrumbItem(text: _plant.Code, href: $"plants/{PlantId}"),
                new BreadcrumbItem(text: _area.Code, href: $"plants/{PlantId}/areas/{AreaId}"),
                new BreadcrumbItem(text: _distribution.Description, href: $"plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}"),
                new BreadcrumbItem(text: Localizer["operationDetails"] +  " / "  + _operation.Description, href: "", disabled: true)
            };
            BreadcrumbService.UpdateBreadcrumbs(_links);
            showui = true;

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

        void GoToDistribution()
        {
            NavigationManager.NavigateTo($"plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}");
        }

        // Update operation
        async void UpdateOperationAsync()
        {
            _operation.IsActive = true;
            var result = await OperationService.UpdateOperation(PlantId, AreaId, DistributionId, OperationId, _operation);

            if (result)
            {
                NavigationManager.NavigateTo($"/plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}");
            }
        }

        void UpdateOperationRedirect(int operationId)
        {
            NavigationManager.NavigateTo($"plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}/operations/updateoperation/{operationId}");
        }

        // Cancel submit form
        void CancelCreateOrUpdate()
        {
            NavigationManager.NavigateTo($"/plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}");
        }

    }
}