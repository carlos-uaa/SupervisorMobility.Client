using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.ProductPage.DistributionPage
{
    public partial class CreateDistribution
    {
        [Parameter]
        public int ProductId { get; set; }

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
        ProductDistribution _distribution = new();

        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            _product = await ProductService.GetProductById(ProductId);
        }

        // Links

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
            await ProductDistributionService.CreateDistribution(ProductId, _distribution);
            NavigationManager.NavigateTo($"products/{ProductId}");
        }
    }
}
