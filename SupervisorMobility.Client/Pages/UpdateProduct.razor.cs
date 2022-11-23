using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class UpdateProduct
    {
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Products", href: "/products"),
            new BreadcrumbItem("Update product", href: "", disabled: true)
        };

        Product _product = new();

        void UpdateProductAsync()
        {
            NavigationManager.NavigateTo($"products");
        }
    }
}
