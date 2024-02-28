using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Pages.Configuration.ProductPage
{
    public partial class Products
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links;

        // Objects
        public List<Product> _products { get; set; } = new();
        Product _product = new();


        // Initialization
        protected async override Task OnInitializedAsync()
        {
            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                new BreadcrumbItem(text: Localizer["ProductsTitle"],  href: "", disabled: true)
            };
            _products = await ProductService.GetProducts();
            BreadcrumbService.UpdateBreadcrumbs(_links);
        }

        // Create product
        void CreateProduct()
        {
            NavigationManager.NavigateTo($"products/createproduct");
        }


        // Delete product
        async Task DeleteProduct(int productId)
        {
            _products.RemoveAll(product => product.ProductId == productId);
            await ProductService.DeleteProduct(productId);

            visibleDelete = false;
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


        //Delete Product
        private bool visibleDelete = false;
        public int deleteProductId = 0;
        private void OpenDeleteDialog(int deleteId)
        {
            deleteProductId = deleteId;
            visibleDelete = true;
        }
        void CloseDeleteModal() => visibleDelete = false;
        private DialogOptions dialogDeleteOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, Position = DialogPosition.TopCenter, DisableBackdropClick = true, CloseButton = true };
      
        private int selectedRowNumber = -1;
        private MudTable<Product> SelectTableEvent;

        private void RowClickEvent(TableRowClickEventArgs<Product> tableRowClickEventArgs)
        {
        }

        private string SelectedRowClassFunc(Product element, int rowNumber)
        {
            if (selectedRowNumber == rowNumber)
            {
                selectedRowNumber = -1;
                if (SelectTableEvent.SelectedItem != null && SelectTableEvent.SelectedItem.Equals(element))
                {
                    NavigationManager.NavigateTo($"products/{element.ProductId}");

                }
                return string.Empty;
            }
            else if (SelectTableEvent.SelectedItem != null && SelectTableEvent.SelectedItem.Equals(element))
            {
                selectedRowNumber = rowNumber;
                return "selected";
            }
            else
            {
                return string.Empty;
            }
        }

    }

}
