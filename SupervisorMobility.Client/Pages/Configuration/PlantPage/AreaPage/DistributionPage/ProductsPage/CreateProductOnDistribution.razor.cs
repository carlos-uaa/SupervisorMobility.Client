using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Pages.Configuration.ProductPage;

namespace SupervisorMobility.Client.Pages.Configuration.PlantPage.AreaPage.DistributionPage.ProductsPage
{
    public partial class CreateProductOnDistribution
    {
        [Parameter]
        public int PlantId { get; set; }
        [Parameter]
        public int AreaId { get; set; }
        [Parameter]
        public int DistributionId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links;
        // Objects
        public Plant _plant { get; set; }
        public Area _area { get; set; }
        public Distribution _distribution { get; set; }
        public Product _product = new();

        private bool showui = false;

        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            _plant = await PlantServices.GetPlantById(PlantId);
            _area = await AreaServices.GetAreaById(PlantId, AreaId);
            _distribution = await DistributionServices.GetDistributionWithCollections(PlantId, AreaId, DistributionId);

            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "#"),
                new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                new BreadcrumbItem(text: Localizer["plants"], href: "/plants"),
                new BreadcrumbItem($"{_plant.Description}", href: $"plants/{PlantId}"),
                new BreadcrumbItem($"{_area.Description}", href: $"plants/{PlantId}/areas/{AreaId}"),
                new BreadcrumbItem($"{_distribution.Description}", href: $"plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}"),
                new BreadcrumbItem(text: Localizer["createDistributionToProduct"], href: "", disabled: true)
            };
            showui = true;

        }


        void GoToProduct()
        {
            NavigationManager.NavigateTo($"plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}");
        }

        // Cancel submit form
        void CancelCreateOrUpdate()
        {
            NavigationManager.NavigateTo($"plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}");
        }


        // Create distribution
        async void CreateProductInDistributionAsync()
        {
            Console.WriteLine($"plant{PlantId}  area{AreaId} dist {DistributionId} prod {_product.ProductId} product {_product.ProductId}");

            var result = await DistributionServices.CreateProduct(PlantId, AreaId, DistributionId, _product);

            if (result != null)
            {
                NavigationManager.NavigateTo($"plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}");
            }
        }
    }
}
