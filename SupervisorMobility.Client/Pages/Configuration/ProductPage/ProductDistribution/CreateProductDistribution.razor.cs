using MudBlazor;
using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Pages.Configuration.ProductPage.ProductDistribution
{
    public partial class CreateProductDistribution
    {
        [Parameter]
        public int ProductId { get; set; }
        // Objects
        Product _product = new();
        Distribution _distribution = new();

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
        };


        protected override async Task OnParametersSetAsync()
        {
            _product = await ProductServices.GetProductAndCollection(ProductId);
            _plants = await PlantServices.GetPlants();

            _links.Add(new BreadcrumbItem($"{_product.Description}", href: $"/products/{ProductId}"));
            _links.Add(new BreadcrumbItem("Create New Distribution", href: "", disabled: true));

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
            
            var result = await ProductServices.CreateDistribution(ProductId, PlantId, AreaId, _distribution);
            
            if(result != null )
            {
                NavigationManager.NavigateTo($"products/{ProductId}");
            }

        }
    }
}
