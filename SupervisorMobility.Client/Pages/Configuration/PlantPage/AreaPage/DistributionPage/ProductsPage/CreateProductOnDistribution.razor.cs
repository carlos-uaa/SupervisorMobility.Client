using MudBlazor;
using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Pages.Configuration.PlantPage.AreaPage.DistributionPage.ProductsPage
{
    public partial class CreateProductOnDistribution
    {
        [Parameter]
        public int ProductId { get; set; }

        public int PlantId { get; set; }
        public int AreaId { get; set; }

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
        Product _product = new();
        Distribution _distribution = new();

        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            _product = await ProductServices.GetProductById(ProductId);
            _plants = await PlantServices.GetPlants();

        }


        async void UpdateAreas()
        {
            AreaId = 0;
            _areas = await AreaServices.GetAreas(PlantId);
        }

        void GoToProduct()
        {
            NavigationManager.NavigateTo($"products/{ProductId}");
        }

        // Cancel submit form
        void CancelCreateOrUpdate()
        {
            NavigationManager.NavigateTo($"products/{ProductId}");
        }

        // Create distribution
        async void CreateDistributionAsync()
        {
            _distribution.IsActive = true;
            var result = await ProductServices.CreateDistribution(ProductId, PlantId, AreaId, _distribution);
            
            if(result != null )
            {
                NavigationManager.NavigateTo($"products/{ProductId}");
            }

        }
    }
}
