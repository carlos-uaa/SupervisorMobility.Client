using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.ProductPage.DistributionPage
{
    public partial class UpdateDistribution
    {
        // Parameters
        [Parameter]
        public int ProductId { get; set; }

        [Parameter]
        public int DistributionId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Products", href: "/products"),
            new BreadcrumbItem("ProductDetail", href: "", disabled: true),
            new BreadcrumbItem("UpdateDist", href: "", disabled: true)
        };

        // Objects
        Product _product = new();
        public ProductDistribution _distribution { get; set; } = new();

        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            _product = await ProductService.GetProductById(ProductId);
            _distribution = await ProductDistributionService.GetDistributionById(ProductId, DistributionId);
        }

        // Links

        void GoToProduct ()
        {
            NavigationManager.NavigateTo($"products/{ProductId}");
        }

        // Update distribution
        void UpdateDistributionAsync()
        {
            ProductDistributionService.UpdateDistribution(ProductId, _distribution);
            NavigationManager.NavigateTo($"products/{ProductId}");
        }

        // Cancel submit form
        void CancelCreateOrUpdate()
        {
            NavigationManager.NavigateTo($"products/{ProductId}");
        }
    }
}
