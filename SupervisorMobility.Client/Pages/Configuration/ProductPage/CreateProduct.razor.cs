using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.ProductPage
{
    public partial class CreateProduct
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Products", href: "/products"),
            new BreadcrumbItem("New product", href: "", disabled: true)
        };

        // Objects
        Product _product = new();

        // Create product
        async void CreateProductAsync()
        {
            var result = await ProductService.CreateProduct(_product);
            NavigationManager.NavigateTo($"products");
        }
    }
}
