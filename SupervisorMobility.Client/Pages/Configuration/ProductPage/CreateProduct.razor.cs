using MudBlazor;
using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Pages.Configuration.ProductPage
{
    public partial class CreateProduct
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links;

        // Objects
        Product _product = new();
        protected async override Task OnInitializedAsync()
        {
            _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(text: Localizer["home"], href: "#"),
            new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
            new BreadcrumbItem(text: Localizer["ProductsTitle"],  href: "/products", disabled: false),
            new BreadcrumbItem(text: Localizer["ProductsNew"],  href: "", disabled: true)
        };

        }
        // Create product
        async void CreateProductAsync()
        {
            _product.IsActive = true;
            var result = await ProductService.CreateProduct(_product);
            NavigationManager.NavigateTo($"products");
        }
    }
}
