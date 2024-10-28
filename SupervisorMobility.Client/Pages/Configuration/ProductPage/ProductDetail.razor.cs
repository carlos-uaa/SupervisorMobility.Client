using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Pages.Configuration.ProductPage
{
    public partial class ProductDetail
    {
        [Parameter]
        public int ProductId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links;

        Product _product = new();

        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            _product = await ProductService.GetProductAndCollection(ProductId);
            _links = new List<BreadcrumbItem>
            {
               new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                new BreadcrumbItem(text: Localizer["ProductsTitle"],  href: "/products", disabled: false),
                new BreadcrumbItem(text: _product.Description, href: "", disabled: true),
            };
            BreadcrumbService.UpdateBreadcrumbs(_links);
        }

        void CreateDistribution()
        {
            NavigationManager.NavigateTo($"products/{ProductId}/CreateDistributionByProducts");
        }

        void AddExistDistribution()
        {
            NavigationManager.NavigateTo($"products/{ProductId}/AddDistributionInProducts");
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
            NavigationManager.NavigateTo($"products/{ProductId}/distribution/{distributionId}");
        }

        // Update distribution
        void UpdateDistribution(int distributionId)
        {
            NavigationManager.NavigateTo($"products/{ProductId}/updatedistribution/{distributionId}");
        }

        // Create product
        void CreateProduct()
        {
            NavigationManager.NavigateTo($"products/createproduct");
        }

        string searchString = string.Empty;

        private bool FilterFunc(Distribution element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.DistributionId.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Code.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{element.DistributionId} {element.Code} {element.Description}".Contains(searchString))
                return true;
            return false;
        }

        private int selectedRowNumber = -1;
        private MudTable<Distribution> SelectTableEvent;

        private void RowClickEvent(TableRowClickEventArgs<Distribution> args)
        {
            var visibleItems = SelectTableEvent.FilteredItems.ToList();
            var rowIndex = visibleItems.IndexOf(args.Item);

            if (selectedRowNumber == rowIndex)
            {
               
                    NavigationManager.NavigateTo($"products/{ProductId}/distribution/{args.Item.DistributionId}");
            }
            else
            {
                SelectTableEvent.SelectedItem = args.Item;
                selectedRowNumber = rowIndex;
                StateHasChanged();
            }
        }

        private string SelectedRowClassFunc(Distribution element, int rowNumber)
        {
            var visibleItems = SelectTableEvent.FilteredItems.ToList();

            if (selectedRowNumber == rowNumber)
            {
                return "selected"; // Marca la fila seleccionada
            }
            else if (SelectTableEvent.SelectedItem != null && SelectTableEvent.SelectedItem.Equals(element))
            {
                selectedRowNumber = visibleItems.IndexOf(element);  // Usa el índice filtrado
                return "selected";
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
