using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.ProductPage
{
    public partial class UpdateProduct
    {
        // Parameters
        [Parameter]
        public int ProductId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "/"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Products", href: "/products"),
            new BreadcrumbItem("Update product", href: "", disabled: true)
        };

        // Objects
        public Product _product { get; set; } = new();

        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            Product dbProduct = await ProductService.GetProductById(ProductId);
            _product = dbProduct;
        }

        // Update product
        async Task UpdateProductAsync()
        {

            var result = await ProductService.UpdateProduct(_product);

            if (result)
            {
                NavigationManager.NavigateTo("/products");
            }

        }
    }
}
