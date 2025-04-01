using DocumentFormat.OpenXml.InkML;
using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.PlantPage.AreaPage.DistributionPage.OperationPage
{
    public partial class UpdateOperation
    {
        // Parameters
        [Parameter]
        public int PlantId { get; set; }

        [Parameter]
        public int AreaId { get; set; }

        [Parameter]
        public int DistributionId { get; set; }

        [Parameter]
        public int OperationId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links;

        // Objects
        Plant _plant = new();
        Area _area = new();
        Distribution _distribution = new();
        public Operation _operation { get; set; } = new();
        private List<Product> _products = new List<Product>();
        private Product _product = new Product();
        private bool showui = false;

        public List<string> NameTimeList = new List<string>();
        public List<string> TimeList = new List<string>();
        public List<string> AdditionalTimeList = new List<string>();
        public List<string> StandardTimeList = new List<string>(); 

        protected override async Task OnParametersSetAsync()
        {
            _plant = await PlantService.GetPlantById(PlantId);
            _area = await AreaService.GetAreaById(PlantId, AreaId);
            _distribution = await DistributionService.GetDistributionById(PlantId, AreaId, DistributionId);
            _operation = await OperationService.GetOperationById(PlantId, AreaId, DistributionId, OperationId);
                _products = await ProductsServices.GetProducts();
            _links = new List<BreadcrumbItem>
                     {
                         new BreadcrumbItem(text: Localizer["home"], href: "/"),
                         new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                         new BreadcrumbItem(text: Localizer["plants"], href: "/plants"),
                         new BreadcrumbItem(text: _plant.Code, href: $"plants/{PlantId}"),
                         new BreadcrumbItem(text: _area.Code, href: $"plants/{PlantId}/areas/{AreaId}"),
                         new BreadcrumbItem(text: _distribution.Description, href: $"plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}"),
                         new BreadcrumbItem(text: Localizer["updateOperation"] +  " / "  + _operation.Description, href: "", disabled: true)
                     };
            BreadcrumbService.UpdateBreadcrumbs(_links);

            if (!string.IsNullOrEmpty(_operation.NameTime))
            {
                NameTimeList = _operation.NameTime.Split('§').ToList();
            }

            if (!string.IsNullOrEmpty(_operation.Time))
            {
                TimeList = _operation.Time.Split('§').ToList();
            }

            if (!string.IsNullOrEmpty(_operation.AdditionalTime))
            {
                AdditionalTimeList = _operation.AdditionalTime.Split('§').ToList();
            }

            if (!string.IsNullOrEmpty(_operation.StandardTime))
            {
                StandardTimeList = _operation.StandardTime.Split('§').ToList();
            }

            ListHasFiveElements(NameTimeList);
            ListHasFiveElements(TimeList);
            ListHasFiveElements(AdditionalTimeList);
            ListHasFiveElements(StandardTimeList);

            _product = _products.Find(p => p.Code == _operation.ProductName);


            showui = true;
        }

        private void ListHasFiveElements(List<string> list)
        {
            while (list.Count < 5)
            {
                list.Add(string.Empty);
            }
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

        void GoToDistribution()
        {
            NavigationManager.NavigateTo($"plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}");
        }

        // Update operation
        async void UpdateOperationAsync()
        {
            // Actualiza los valores de _operation con los valores de los arrays
            _operation.NameTime = string.Join('§', NameTimeList);
            _operation.Time = string.Join('§', TimeList);
            _operation.AdditionalTime = string.Join('§', AdditionalTimeList);
            _operation.StandardTime = string.Join('§', StandardTimeList);

            _operation.IsActive = true;
            var result = await OperationService.UpdateOperation(PlantId, AreaId, DistributionId, OperationId, _operation);

            if (result)
            {
                NavigationManager.NavigateTo($"/plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}");
            }
        }

        // Cancel submit form
        void CancelCreateOrUpdate()
        {
            NavigationManager.NavigateTo($"/plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}");
        }

        void UpdateProduct()
        {
           
                _product = _products.Find(p => p.Code == _operation.ProductName);
        
            StateHasChanged();
        }
    }
}
