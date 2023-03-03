using Microsoft.JSInterop;
using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.ProductPage
{
    public partial class ProductDetail
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
        };

        Product _product = new();

        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            _product = await ProductService.GetProductAndCollection(ProductId);
        }

        void CreateDistribution()
        {
            NavigationManager.NavigateTo($"product/{ProductId}/distributions/createdistribution");
        }

        // Delete distribution
        async Task DeleteDistribution(int distributionId)
        {
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this distribution?");

            if (confirm)
            {
                _product.Distributions.RemoveAll(distribution => distribution.DistributionId == distributionId);
                await ProductService.DeleteDistribution(ProductId, distributionId);
            }
        }

        // Distribution details
        void DistributionDetails(int distributionId)
        {
            NavigationManager.NavigateTo($"product/{ProductId}/distributions/{distributionId}");
        }

        // Update distribution
        void UpdateDistribution(int distributionId)
        {
            NavigationManager.NavigateTo($"product/{ProductId}/updatedistribution/{distributionId}");
        }
    }
}
