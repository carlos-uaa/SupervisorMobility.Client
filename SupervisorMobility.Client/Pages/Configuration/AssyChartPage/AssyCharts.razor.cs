using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.CDMS;
using SupervisorMobility.Client.Data.Entities.CDMS.Documents;
using SupervisorMobility.Client.Pages.Configuration.PlantPage;
using SupervisorMobility.Client.Services.AssyChartService;
using static MudBlazor.CategoryTypes;

namespace SupervisorMobility.Client.Pages.Configuration.AssyChartPage
{
    public partial class AssyCharts
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Assy Chart", href: "/assychart", disabled: true),
        };


        //Objects
        private bool hover = true;
        private bool ronly = false;
        private string searchString = "";

        private string HOErute = "";
        private string CCPrute = "";
        private string GOSrute = "";
        public List<AssyChart> _assychart { get; set; } = new();
        public List<AssyChart> _assychartplant { get; set; } = new();
        public List<AssyChart> _assychartarea { get; set; } = new();
        public List<AssyChart> _assychartdistribution { get; set; } = new();

        List<Plant> _plants { get; set; } = new();
        List<Area> _areas = new();
        List<Distribution> _distributions { get; set; } = new();

        bool seeplant = true;
        bool seearea = false;
        bool seedistribution = false;

        bool showtablePlant = false;
        bool showtableArea = false;
        bool showtableDistribution = false;

        private int plantId = 0;
        private int areaId = 0;
        private int distributionId = 0;

        private bool CcpDialog = false;
        private bool HoeDialog = false;
        private bool GosDialog = false;

        private CDMS_CCP_Document? CcpFilesInFolder;
        private CDMS_HOE_Document? HoeFilesInFolder;
        private CDMS_GOS_Document? GosFilesInFolder;


        private bool folderError = false;

        protected async override Task OnInitializedAsync()
        {
            _assychart = await AssyChartServices.GetAssyCharts();
            _plants = await PlantServices.GetPlants();
        }

        async void UpdateAreas()
        {
            _assychartplant.Clear();
            showtablePlant = false;
            distributionId = 0;
            _distributions.Clear();
            areaId = 0;
            _areas.Clear();
            _areas = await AreaServices.GetAreas(plantId);
            _assychartplant = await AssyChartServices.GetAssyChartsByPlant(plantId);
            showtablePlant = true;
            StateHasChanged();
        }
        //Function Update Distributions on change Area select

        private async void UpdateDistributions()
        {
            _assychartarea.Clear();
            showtableArea = false;
            distributionId = 0;
            _distributions.Clear();
            _distributions = await DistributionServices.GetDistributions(plantId, areaId);
            _assychartarea = await AssyChartServices.GetAssyChartsByArea(plantId, areaId);
            showtableArea = true;
            StateHasChanged();
        }

        private async void UpdateDistirbutionAssyChart()
        {
            _assychartdistribution.Clear();
            showtableDistribution = false;
            _assychartdistribution = await AssyChartServices.GetAssyChartsByDistribution(plantId, areaId, distributionId);
            showtableDistribution = true;
            StateHasChanged();
        }

        private void plantsTab()
        {
            seeplant = true;
            seearea = false;
            seedistribution = false;
        }
        private void areaTab()
        {
            seeplant = true;
            seearea = true;
            seedistribution = false;
        }
        private void distributionTab()
        {
            seeplant = true;
            seearea = true;
            seedistribution = true;
        }



        //Filtering

        private bool FilterFunc(AssyChart element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.Plant.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Area.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.GOS.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.CCP.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.HOE.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Operation.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Product.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.CreationDate.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.ModificationDate.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{element.GOS}{element.CCP}{element.Operation.Description}{element.Plant.Description}{element.Area.Description}{element.Distribution.Description}".Contains(searchString))
                return true;
            return false;
        }

        private TableGroupDefinition<AssyChart> _groupDefinition = new()
        {
            GroupName = "Group",
            Selector = (e) => e.Product.Description
        };

        void GoToUpdateAssyChart(int assychartid)
        {
            NavigationManager.NavigateTo($"assychart/updateassychart/{assychartid}");
        }

        async Task DeleteAssyChart(int assychartid)
        {

            bool confirm = await JS.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this AssyChart?");

            if (confirm)
            {
                _assychart.RemoveAll(assychart => assychart.AssyChardId == assychartid);
                await AssyChartServices.DeleteAssyChart(assychartid);
            }



        }


        
        private async void OpenDialogGOS(string ruta)
        {
            GOSrute = ruta;
            GosDialog = true;
            folderError = false;

            Console.WriteLine($"gos {ruta}");

            GosFilesInFolder = new CDMS_GOS_Document();

            GosFilesInFolder = await CDMSServices.GetFilesGOS(ruta);
            if (GosFilesInFolder == null)
                folderError = true;


            StateHasChanged();
        }
        void CloseGos() => GosDialog = false;

        private async void OpenDialogCcp(string ruta)
        {
            CCPrute= ruta;
            CcpDialog = true;
            folderError = false;
            Console.WriteLine($"Cpc {ruta}");

            CcpFilesInFolder = new CDMS_CCP_Document();
            CcpFilesInFolder = await CDMSServices.GetFilesCCP(ruta);
            if (CcpFilesInFolder == null)
                folderError = true;

            StateHasChanged();
        }
        void CloseCcp() => CcpDialog = false;

        private async void OpenDialogHoe(string ruta)
        {
            HOErute = ruta;
            HoeDialog = true;
            Console.WriteLine($"hoe {ruta}");

            folderError = false;
            HoeFilesInFolder = new CDMS_HOE_Document();
            HoeFilesInFolder = await CDMSServices.GetFilesHOE(ruta);
            if (HoeFilesInFolder == null)
                folderError = true;

            StateHasChanged();
        }
        void CloseHoe() => HoeDialog = false;

        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };

        private async Task DownloadFileFromURL(string urlroute, string namefile)
        {
            var fileName = namefile;
            var fileURL = urlroute;
            await JS.InvokeVoidAsync("triggerFileDownload", fileName, fileURL);
        }
    }
}
