using Microsoft.JSInterop;
using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.ProductPage
{
    public partial class Products
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Products", href: "", disabled: true)
        };

        // Objects
        public List<Product> _products { get; set; } = new();
        Product _product = new();


        // Initialization
        protected async override Task OnInitializedAsync()
        {
            _products = await ProductService.GetProducts();
        }
        
        // Create product
        void CreateProduct()
        {
            NavigationManager.NavigateTo($"products/createproduct");
        }


        // Delete product
        async Task DeleteProduct(int productId)
        {
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this operation?");

            if (confirm)
            {
                _products.RemoveAll(product => product.ProductId == productId);
                await ProductService.DeleteProduct(productId);
            }
        }

        // Update product
        void EditProduct(int productId)
        {
            NavigationManager.NavigateTo($"products/updateproduct/{productId}");
        }

        void ProductDetails(int productId)
        {
            NavigationManager.NavigateTo($"products/{productId}");
        }
        private string searchString = "";

        private bool FilterFunc(Product element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.ProductId.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Code.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true; 
            if (element.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{element.ProductId} {element.Code} {element.Description}".Contains(searchString))
                return true;
            return false;
        }
    }
}
