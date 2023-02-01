using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.ProductPage.DistributionPage.OperationPage
{
    public partial class CreateOperation
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
            new BreadcrumbItem("DistributionDetail", href: "", disabled: true),
            new BreadcrumbItem("New Operation", href: "", disabled: true)
        };

        // Objects
        Product _product = new();
        ProductDistribution _distribution = new();
        ProductOperation _operation = new();

        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            _product = await ProductService.GetProductById(ProductId);
            _distribution = await ProductDistributionService.GetDistributionById(ProductId, DistributionId);
        }

        // Links
        void GoToProducts()
        {
            NavigationManager.NavigateTo($"products/{ProductId}");
        }

        void GoToDistribution()
        {
            NavigationManager.NavigateTo($"product/{ProductId}/distributions/{DistributionId}");
        }

        // Create operation
        async void CreateOperationAsync()
        {
            var result = await ProductOperationService.CreateOperation(ProductId, DistributionId, _operation);
            NavigationManager.NavigateTo($"product/{ProductId}/distributions/{DistributionId}");
        }

        // Cancel submit form
        void CancelCreateOrUpdate()
        {
            NavigationManager.NavigateTo($"products/{ProductId}");
        }
    }
}
