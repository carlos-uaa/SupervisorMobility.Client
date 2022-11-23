using Microsoft.JSInterop;
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

        public List<Product> _products { get; set; } = new();
        Product _product = new();

        protected async override Task OnInitializedAsync()
        {
            _products = await ProductService.GetProducts();
        }

        void EditProduct(int productId)
        {
            NavigationManager.NavigateTo($"products/updateproduct/{productId}");
        }
        void CreateProduct()
        {
            NavigationManager.NavigateTo($"products/createproduct");
        }

        async Task DeleteProduct(int productId)
        {
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this operation?");

            if (confirm)
            {
                _products.RemoveAll(product => product.ProductId == productId);
                await ProductService.DeleteProduct(productId);
            }
        }
    }
}
