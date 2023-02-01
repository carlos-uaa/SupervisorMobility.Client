using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.ProductPage.DistributionPage.OperationPage
{
    public partial class UpdateOperation
    {
        [Parameter]
        public int ProductId { get; set; }

        [Parameter]
        public int DistributionId { get; set; }

        [Parameter]
        public int OperationId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Products", href: "/products"),
            new BreadcrumbItem("ProductDetail", href: "", disabled: true),
            new BreadcrumbItem("DistributionDetail", href: "", disabled: true),
            new BreadcrumbItem("UpdateOperation", href: "", disabled: true)
        };


        // Objects
        // Objects
        Product _product = new();
        ProductDistribution _distribution = new();
        ProductOperation _operation = new();


        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            _product = await ProductService.GetProductById(ProductId);
            _distribution = await ProductDistributionService.GetDistributionById(ProductId, DistributionId);
            _operation = await ProductOperationService.GetOperationById(ProductId, DistributionId, OperationId);
        }

        // Links

        void GoToProduct()
        {
            NavigationManager.NavigateTo($"products/{ProductId}");
        }

        void GoToDistribution()
        {
            NavigationManager.NavigateTo($"product/{ProductId}/distributions/{DistributionId}");
        }

        // Update operation
        async void UpdateOperationAsync()
        {
            await ProductOperationService.UpdateOperation(ProductId, DistributionId, OperationId, _operation);
            NavigationManager.NavigateTo($"product/{ProductId}/distributions/{DistributionId}");
        }

        // Cancel submit form
        void CancelCreateOrUpdate()
        {
            NavigationManager.NavigateTo($"product/{ProductId}/distributions/{DistributionId}");
        }
    }
}
