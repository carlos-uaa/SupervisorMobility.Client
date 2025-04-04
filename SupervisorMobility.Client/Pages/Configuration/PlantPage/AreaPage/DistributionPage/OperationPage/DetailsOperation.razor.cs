using MudBlazor;
using SupervisorMobility.Client.Data.Entities.TreeStruct;
using SupervisorMobility.Client.Services.ProductsService;

namespace SupervisorMobility.Client.Pages.Configuration.PlantPage.AreaPage.DistributionPage.OperationPage
{
    public partial class DetailsOperation
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
        private bool showui = false;

        public Dictionary<string,List<string>> NameTimeList = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> TimeList = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> AdditionalTimeList = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> StandardTimeList = new Dictionary<string, List<string>>();
        private List<Product> _products = new List<Product>();
        private Product _product = new Product();

        private List<string> openTabs = new List<string>();
        private int activeTabIndex = 0;
        protected async override Task OnInitializedAsync()
        {
            _plant = await PlantService.GetPlantById(PlantId);
            _area = await AreaService.GetAreaById(PlantId, AreaId);
            _distribution = await DistributionService.GetDistributionById(PlantId, AreaId, DistributionId);
            _operation = await OperationService.GetOperationById(PlantId, AreaId, DistributionId, OperationId);

            if(!string.IsNullOrEmpty(_operation.ProductName))
                openTabs = _operation.ProductName.Split('§').ToList();


            try
            {
                if (!string.IsNullOrEmpty(_operation.NameTime))
                {
                    var nameTimeDict = JsonSerializer.Deserialize<Dictionary<string, string>>(_operation.NameTime);
                    foreach (var kvp in nameTimeDict)
                    {
                        NameTimeList[kvp.Key] = kvp.Value.Split('§').ToList();
                    }
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializando NameTime: {ex.Message}");
                NameTimeList = new Dictionary<string, List<string>>(); // Inicializar con valores genéricos
                foreach(var kvp in openTabs)
                {
                    NameTimeList[kvp] = new List<string>();
                }
            }

            try
            {
                if (!string.IsNullOrEmpty(_operation.Time))
                {
                    var TimeDict = JsonSerializer.Deserialize<Dictionary<string, string>>(_operation.Time);
                    foreach (var kvp in TimeDict)
                    {
                        TimeList[kvp.Key] = kvp.Value.Split('§').ToList();
                    }
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializando Time: {ex.Message}");
                TimeList = new Dictionary<string, List<string>>(); // Inicializar con valores genéricos
                foreach (var kvp in openTabs)
                {
                    TimeList[kvp] = new List<string>();
                }
            }

            try
            {
                if (!string.IsNullOrEmpty(_operation.AdditionalTime))
                {
                    var AditionalTimeDict = JsonSerializer.Deserialize<Dictionary<string, string>>(_operation.AdditionalTime);
                    foreach (var kvp in AditionalTimeDict)
                    {
                        AdditionalTimeList[kvp.Key] = kvp.Value.Split('§').ToList();
                    }
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializando AdditionalTime: {ex.Message}");
                AdditionalTimeList = new Dictionary<string, List<string>>(); // Inicializar con valores genéricos
                foreach (var kvp in openTabs)
                {
                    AdditionalTimeList[kvp] = new List<string>();
                }
            }

            try
            {
                if (!string.IsNullOrEmpty(_operation.StandardTime))
                {
                    var StandardTimeDict = JsonSerializer.Deserialize<Dictionary<string, string>>(_operation.StandardTime);
                    foreach (var kvp in StandardTimeDict)
                    {
                        StandardTimeList[kvp.Key] = kvp.Value.Split('§').ToList();
                    }
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializando StandardTime: {ex.Message}");
                StandardTimeList = new Dictionary<string, List<string>>(); // Inicializar con valores genéricos
                foreach (var kvp in openTabs)
                {
                    StandardTimeList[kvp] = new List<string>();
                }
            }

            _products = await ProductsServices.GetProducts();

            if (!string.IsNullOrEmpty(_operation.ProductName))
                _product = _products.Find(p => p.Code == openTabs.FirstOrDefault().ToString());


            ListHasFiveElements(NameTimeList);
            ListHasFiveElements(TimeList);
            ListHasFiveElements(AdditionalTimeList);
            ListHasFiveElements(StandardTimeList);


            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                new BreadcrumbItem(text: Localizer["plants"], href: "/plants"),
                new BreadcrumbItem(text: _plant.Code, href: $"plants/{PlantId}"),
                new BreadcrumbItem(text: _area.Code, href: $"plants/{PlantId}/areas/{AreaId}"),
                new BreadcrumbItem(text: _distribution.Description, href: $"plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}"),
                new BreadcrumbItem(text: Localizer["operationDetails"] +  " / "  + _operation.Description, href: "", disabled: true)
            };
            BreadcrumbService.UpdateBreadcrumbs(_links);
            showui = true;

        }

        private void ListHasFiveElements(Dictionary<string, List<string>> list)
        {
            foreach (var kvp in list)
            {
                while (kvp.Value.Count < 5)
                {
                    kvp.Value.Add(string.Empty);
                }
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
            _operation.IsActive = true;
            var result = await OperationService.UpdateOperation(PlantId, AreaId, DistributionId, OperationId, _operation);

            if (result)
            {
                NavigationManager.NavigateTo($"/plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}");
            }
        }

        void UpdateOperationRedirect(int operationId)
        {
            NavigationManager.NavigateTo($"plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}/operations/updateoperation/{operationId}");
        }

        // Cancel submit form
        void CancelCreateOrUpdate()
        {
            NavigationManager.NavigateTo($"/plants/{PlantId}/areas/{AreaId}/distributions/{DistributionId}");
        }
        void UpdateProduct()
        {
            string code = openTabs.ElementAt(activeTabIndex).ToString();
            _product = _products.Find(p => p.Code == code);

            StateHasChanged();
        }
    }
}