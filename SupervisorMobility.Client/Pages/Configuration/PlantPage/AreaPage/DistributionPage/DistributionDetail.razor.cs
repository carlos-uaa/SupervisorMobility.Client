using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Pages.Configuration.PlantPage.AreaPage.DistributionPage
{
    public partial class DistributionDetail
    {
        // Parameters
        [Parameter]
        public int PlantId { get; set; }

        [Parameter]
        public int AreaId { get; set; }

        [Parameter]
        public int DistributionId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links;

        // Objects
        Plant _plant = new();
        Area _area = new();
        Distribution _distribution = new();

        // Initialization
        protected async override Task OnInitializedAsync()
        {
            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "#"),
                new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                new BreadcrumbItem(text: Localizer["plants"], href: "/plants"),
                new BreadcrumbItem(text: Localizer["plantDetails"], href: ""),
                new BreadcrumbItem(text: Localizer["areaDetails"], href: ""),
                new BreadcrumbItem(text: Localizer["distributionDetails"], href: "", disabled: true)
            };
        }

        protected override async Task OnParametersSetAsync()
        {
            _distribution = await DistributionService.GetDistributionWithCollections(PlantId, AreaId, DistributionId);
            _area = await AreaService.GetOneAreaIncludingCollections(PlantId, AreaId);
            _plant = await PlantService.GetPlantById(PlantId);
        }

        // Links
        void GoToPlant()
        {
            NavigationManager.NavigateTo($"plants/{PlantId}");
        }

        void GoToArea()
        {
            NavigationManager.NavigateTo($"plants/{PlantId}/areas/{AreaId}");
        }

        // Create operation
        void CreateOperation()
        {
            NavigationManager.NavigateTo($"plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}/createoperation");
        }

        void CreateProduct()
        {
            NavigationManager.NavigateTo($"plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}/CreateProductInDistribution");
        }

        void AddExistProduct()
        {
            NavigationManager.NavigateTo($"plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}/AddProductInDistribution");
        }

        // Delete operation
        async Task DeleteOperation(int operationId)
        {
            _distribution.Operations.RemoveAll(operation => operation.OperationId == operationId);
            await OperationService.DeleteOperation(PlantId, AreaId, DistributionId, operationId);

            visibleDelete = false;
        }
        
        async Task DeleteProduct(int productId)
        {
            _distribution.Products.RemoveAll(product => product.ProductId == productId);
            await DistributionService.DeleteProductFromDistribution(PlantId, AreaId, DistributionId, productId);

            visibleDeleteProduct = false;
        }

        // Update operation
        void UpdateOperation(int operationId)
        {
            NavigationManager.NavigateTo($"plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}/operations/updateoperation/{operationId}");
        }

        // Update product
        void EditProduct(int productId)
        {
            NavigationManager.NavigateTo($"/plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}/product/{productId}/update");
        }
        void ProductDetails(int productId)
        {
            NavigationManager.NavigateTo($"/plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}/product/{productId}/details");
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
        private bool FilterOperation(Operation element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.OperationId.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Code.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{element.OperationId} {element.Code} {element.Description}".Contains(searchString))
                return true;
            return false;
        }


        //Delete Operation 
        private bool visibleDelete = false;
        public int deleteOperationId = 0;
        private void OpenDeleteDialog(int deleteId)
        {
            deleteOperationId = deleteId;
            visibleDelete = true;
        }
        void CloseDeleteModal() => visibleDelete = false;
        private DialogOptions dialogDeleteOptions = new() {CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, Position = DialogPosition.TopCenter, DisableBackdropClick = true, CloseButton = true };

        //Delete Operation 
        private bool visibleDeleteProduct = false;
        public int deleteProductId = 0;
        private void OpenDeleteProductDialog(int deleteId)
        {
            deleteProductId = deleteId;
            visibleDeleteProduct = true;
        }
        void CloseDeleteProductModal() => visibleDeleteProduct = false;
        private DialogOptions dialogDeleteProductOptions = new() {CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, Position = DialogPosition.TopCenter, DisableBackdropClick = true, CloseButton = true };
    }
}
