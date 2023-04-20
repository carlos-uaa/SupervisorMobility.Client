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

        public List<Plant> _plants { get; set; } = new List<Plant>();
        public List<Area> _areas { get; set; } = new List<Area>();

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Products", href: "/products"),
            new BreadcrumbItem("ProductsDetail", href: "", disabled: true),
            new BreadcrumbItem("New Distribution", href: "", disabled: true),
        };

        // Objects
        public Plant _plant { get; set; }
        public Area _area { get; set; }
        public Distribution _distribution { get; set; }
        public Product _product = new();
        public List<Product> _products { get; set; } = new List<Product>();


        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            _products = await ProductServices.GetProducts();
            _plant = await PlantServices.GetPlantById(PlantId);
            _area = await AreaServices.GetAreaById(PlantId, AreaId);
            _distribution = await DistributionServices.GetDistributionWithCollections(PlantId, AreaId, DistributionId);


            _links.Add(new BreadcrumbItem($"{_plant.Description}", href: $"plants/{PlantId}"));
            _links.Add(new BreadcrumbItem($"{_area.Description}", href: $"plants/{PlantId}/areas/{AreaId}"));
            _links.Add(new BreadcrumbItem($"{_distribution.Description}", href: $"plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}"));
            _links.Add(new BreadcrumbItem("Add Existent Product", href: "", disabled: true));

        }


        async void UpdateAreas()
        {
            AreaId = 0;
            _areas = await AreaServices.GetAreas(PlantId);
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
        async void CreateDistributionAsync()
        {
            Console.WriteLine($"plant{PlantId}  area{AreaId} dist {DistributionId} prod {_product.ProductId} product {_product.ProductId}");

            var result = await DistributionServices.AddProduct(PlantId, AreaId, DistributionId, _product);

            if (result != null)
            {
                NavigationManager.NavigateTo($"plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}");
            }
        }
    }
}
