using Microsoft.JSInterop;
using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.ProductPage.DistributionPage
{
    public partial class DistributionDetail
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
            new BreadcrumbItem("DistributionDetail", href: "", disabled: true)
        };

        // Objects
        Product _product = new();
        ProductDistribution _distribution = new();
        private List<ProductOperation> _operations = new();

        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            _distribution = await ProductDistributionService.GetDistributionById(ProductId, DistributionId);
            _product = await ProductService.GetProductById(ProductId);
            _operations = await ProductOperationService.GetOperations(ProductId, DistributionId);
        }

        // Links
        void GoToProduct()
        {
            NavigationManager.NavigateTo($"products/{ProductId}");
        }

        // Create operation
        void CreateOperation()
        {
            NavigationManager.NavigateTo($"product/{ProductId}/distributions/{DistributionId}/createoperation");
        }

        // Delete operation
        async Task DeleteOperation(int operationId)
        {
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this operation?");

            if (confirm)
            {
                _operations.RemoveAll(operation => operation.ProductOperationId == operationId);
                await ProductOperationService.DeleteOperation(ProductId, DistributionId, operationId);
            }
        }

        // Update operation
        void UpdateOperation(int operationId)
        {
            NavigationManager.NavigateTo($"product/{ProductId}/distributions/{DistributionId}/operations/updateoperation/{operationId}");
        }


    }
}
