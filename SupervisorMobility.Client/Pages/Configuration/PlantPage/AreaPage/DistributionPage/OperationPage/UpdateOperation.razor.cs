using Blazorise.Extensions;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2013.Drawing.TimeSlicer;
using DocumentFormat.OpenXml.Presentation;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Pages.Inicio.JobObservationPage.Modals;

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

        public Dictionary<string, List<string>> NameTimeList = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> TimeList = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> AdditionalTimeList = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> StandardTimeList = new Dictionary<string, List<string>>();


        private List<string> openTabs = new List<string>();
        private int activeTabIndex = 0;
        protected override async Task OnInitializedAsync()
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


            if (!string.IsNullOrEmpty(_operation.ProductName))
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
                foreach (var kvp in openTabs)
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
            ListHasFiveElements(NameTimeList);
            ListHasFiveElements(TimeList);
            ListHasFiveElements(AdditionalTimeList);
            ListHasFiveElements(StandardTimeList);

            if (!string.IsNullOrEmpty(_operation.ProductName))
                _product = _products.Find(p => p.Code == openTabs.FirstOrDefault());



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
            _operation.ProductName = string.Join("§", openTabs);
            // Actualiza los valores de _operation con los valores de los arrays
            Dictionary<string, string> NameTimeDict = new Dictionary<string, string>();
            Dictionary<string, string> TimesDict = new Dictionary<string, string>();
            Dictionary<string, string> AdditionalTimeDict = new Dictionary<string, string>();
            Dictionary<string, string> StandarTimeDict = new Dictionary<string, string>();

            foreach (var kvp in NameTimeList)
            {
                NameTimeDict[kvp.Key] = string.Join("§", kvp.Value);
            }

            foreach (var kvp in TimeList)
            {
                TimesDict[kvp.Key] = string.Join("§", kvp.Value);
            }

            foreach (var kvp in AdditionalTimeList)
            {
                AdditionalTimeDict[kvp.Key] = string.Join("§", kvp.Value);
            }

            foreach (var kvp in StandardTimeList)
            {
                StandarTimeDict[kvp.Key] = string.Join("§", kvp.Value);
            }

            string NameTimeJson = Newtonsoft.Json.JsonConvert.SerializeObject(NameTimeDict, Newtonsoft.Json.Formatting.Indented);
            string TimesJson = Newtonsoft.Json.JsonConvert.SerializeObject(TimesDict, Newtonsoft.Json.Formatting.Indented);
            string AditioanalTimeJson = Newtonsoft.Json.JsonConvert.SerializeObject(AdditionalTimeDict, Newtonsoft.Json.Formatting.Indented);
            string StandarTimeJson = Newtonsoft.Json.JsonConvert.SerializeObject(StandarTimeDict, Newtonsoft.Json.Formatting.Indented);

            _operation.NameTime = NameTimeJson;
            _operation.Time = TimesJson;
            _operation.AdditionalTime = AditioanalTimeJson;
            _operation.StandardTime = StandarTimeJson;

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
            string code = openTabs.ElementAt(activeTabIndex).ToString();
            _product = _products.Find(p => p.Code == code);

            StateHasChanged();
        }



        IDialogReference dialogModelSelect;
        private DialogOptions dialogModelSelectOptions = new() { CloseOnEscapeKey = false, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, DisableBackdropClick = true, CloseButton = true };
        public string newModel { get; set; }
        async void AddTabCallback()
        {
            var filteredProducts = _products.Where(p => !openTabs.Contains(p.Code)).ToList();
            var parameters = new DialogParameters
                {
                    { "_products", filteredProducts },
                    { "_model", newModel },
                    { "AddModelSelected", EventCallback.Factory.Create<string>(this, AddModelSelected) }
                };
            dialogModelSelect = await DialogService.ShowAsync<ModelSelect_Dialog>("", parameters, dialogModelSelectOptions);

            await dialogModelSelect.Result;
        }


        void RemoveTabCallback(MudTabPanel panel) => RemoveTabProduct(panel.ID.ToString());

        public async Task AddModelSelected(string model)
        {
            if (!model.IsNullOrEmpty())
            {
                dialogModelSelect.Close();

                openTabs.Add(model);

                NameTimeList[model] = new List<string>();
                TimeList[model] = new List<string>();
                AdditionalTimeList[model] = new List<string>();
                StandardTimeList[model] = new List<string>();

                ListHasFiveElements(NameTimeList);
                ListHasFiveElements(TimeList);
                ListHasFiveElements(AdditionalTimeList);
                ListHasFiveElements(StandardTimeList);

                UpdateProduct();

                activeTabIndex = openTabs.Count - 1; // Automatically switch to the new tab.
            }
            StateHasChanged();
        }

        public void RemoveTabProduct(string id)
        {
            Console.WriteLine(id);

            var tabView = openTabs.SingleOrDefault((t) => Equals(t, id));
            if (tabView is not null)
            {
                NameTimeList.Remove(id);
                TimeList.Remove(id);
                AdditionalTimeList.Remove(id);
                StandardTimeList.Remove(id);

                openTabs.Remove(tabView);
                StateHasChanged();
            }
        }

    }
}
