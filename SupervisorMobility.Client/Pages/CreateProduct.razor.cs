using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class CreateProduct
    {
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Products", href: "/products"),
            new BreadcrumbItem("New product", href: "", disabled: true)
        };

        Product _product = new();

        void CreateProductAsync()
        {
            NavigationManager.NavigateTo($"products");
        }
    }
}
