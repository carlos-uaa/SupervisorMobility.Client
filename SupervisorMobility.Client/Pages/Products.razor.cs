using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class Products
    {
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Products", href: "", disabled: true)
        };

        private List<Product> _products = new List<Product>
        {
            new Product { ProductId = 1, Code = "P71A", Description = "Infiniti P71A", IsActive = true },
            new Product { ProductId = 2, Code = "V177", Description = "Mercedes V177", IsActive = false },
            new Product { ProductId = 3, Code = "X247", Description = "Mercedes X247", IsActive = true },
            new Product { ProductId = 4, Code = "N71A", Description = "Infiniti N71A", IsActive = true },
        };

        void EditProduct()
        {
            NavigationManager.NavigateTo($"products/updateproduct");
        }
        void CreateProduct()
        {
            NavigationManager.NavigateTo($"products/createproduct");
        }
    }
}
