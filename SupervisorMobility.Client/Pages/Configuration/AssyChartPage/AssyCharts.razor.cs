
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.TreeStruct;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.Client.Pages.Configuration.AssyChartPage
{
    public partial class AssyCharts
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links;

        //Objects
        private bool hover = true;
        private bool ronly = false;
        private string searchString = "";
        private string searchCodeString = "";

        public List<AssyChart>? _assychart { get; set; } = null;
        public List<AssyChart> _assychartItems { get; set; } = new();

        List<Plant> _plants { get; set; } = new();
        List<Area> _areas = new();
        List<Distribution> _distributions { get; set; } = new();

        bool seeplant = true;
        bool seearea = false;
        bool seedistribution = false;



        bool Showfilters = false;


        bool ShowLoading = true;
        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };


        //Files Path
        private CDMS_CCP_Archives? CcpFilesInFolder;
        private CDMS_HOE_Archives? HoeFilesInFolder;
        private CDMS_GOS_Archives? GosFilesInFolder;
        private CDMS_CCP_Archives? AuxCcpFilesInFolder;
        private CDMS_HOE_Archives? AuxHoeFilesInFolder;
        private CDMS_GOS_Archives? AuxGosFilesInFolder;
        //Error Display Rutes Select ONLY
        private bool folderCCPError = false;
        private bool folderHOEError = false;
        private bool folderGOSError = false;
        //Display Files Errors
        private bool folderErrorGOS = false;
        private bool folderErrorCCP = false;
        private bool folderErrorHOE = false;
        private string HOErute = "";
        private string CCPrute = "";
        private string GOSrute = "";
        //CommonDirection
        private bool folderErrorGOSCD = false;
        private bool folderErrorCCPCD = false;
        private bool folderErrorHOECD = false;
        private string HOEruteCD = "";
        private string CCPruteCD = "";
        private string GOSruteCD = "";
        //CommonDirection Files
        private CDMS_CCP_Archives? CcpFilesInFolderCD;
        private CDMS_HOE_Archives? HoeFilesInFolderCD;
        private CDMS_GOS_Archives? GosFilesInFolderCD;
        private CDMS_CCP_Archives? AuxCcpFilesInFolderCD;
        private CDMS_HOE_Archives? AuxHoeFilesInFolderCD;
        private CDMS_GOS_Archives? AuxGosFilesInFolderCD;


        
        private int plantId = 0;
        private bool if_pick_Plant = false;
        private int areaId = 0;
        private bool if_pick_Area = false;
        private int distributionId = 0;
        private bool if_pick_Distribution = false;
        private int productId = 0;
        public int idFilter;

        MudTabs FilesViewer;
        MudTabPanel HOE;
        MudTabPanel HOECD;
        MudTabPanel CCP;
        MudTabPanel CCPCD;
        MudTabPanel GOS;
        MudTabPanel GOSCD;

        public bool CodePathModalDisplay { get; set; } = false;
        SOSCodePath CodePathDialogDisplay { get; set; }

        private bool folderError = false;

        protected async override Task OnInitializedAsync()
        {
            _sourceMsgLoading.Add($"{Localizer1["Loading1"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading2"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading3"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading4"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading5"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading6"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading7"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading8"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading9"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading10"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading11"]}");
            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                new BreadcrumbItem(text: Localizer["assychart"], href: "/assychart", disabled: true),
            };

            try
            {
                _assychart = await AssyChartServices.GetAssyCharts();
                _assychartItems = ObjectCloner.ObjectCloner.DeepClone(_assychart);

                _plants = await PlantServices.GetPlants();

                foreach (var item in _assychart)
                {
                    if (!_areas.Any(a => a.AreaId == item.AreaId))
                    {
                        _areas.Add(item.Area);
                    }

                    if (!_distributions.Any(d => d.DistributionId == item.DistributionId))
                    {
                        _distributions.Add(item.Distribution);
                    }
                }

                

            }
            catch (Exception ex)
            {

            }
            finally
            {
                ShowLoading = false;
            }

        }

        public void ActiveFilters()
        {
            Showfilters = !Showfilters;

            foreach (var item in _assychartItems)
            {
                if (!_areas.Any(a => a.AreaId == item.AreaId))
                {
                    _areas.Add(item.Area);
                }

                if (!_distributions.Any(d => d.DistributionId == item.DistributionId))
                {
                    _distributions.Add(item.Distribution);
                }
            }
        }

        public async void ClearFilters()
        {
            idFilter = 0;
            plantId = 0;
            areaId = 0;
            distributionId = 0;
            if_pick_Distribution = false;
            productId = 0;
            StateHasChanged();

            await Filters();
            StateHasChanged();

            foreach (var item in _assychartItems)
            {
                if (!_areas.Any(a => a.AreaId == item.AreaId))
                {
                    _areas.Add(item.Area);
                }

                if (!_distributions.Any(d => d.DistributionId == item.DistributionId))
                {
                    _distributions.Add(item.Distribution);
                }
            }


            //await Filters();
        }



        public void ShowError(string txt)
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            Snackbar.Add($"{txt}", Severity.Error);
        }

        async void UpdateAreas_FilterByPlant()
        {
            ShowLoading = true;
            StateHasChanged();
            try
            {
                if_pick_Area = false;
                if_pick_Distribution = false;

                _distributions.Clear();

                areaId = 0;
                distributionId = 0;

                _areas.Clear();


                if (plantId != 0)
                {

                    await Filters();

                    foreach (var item in _assychart)
                    {
                        if (!_areas.Any(a => a.AreaId == item.AreaId))
                        {
                            _areas.Add(item.Area);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                ShowLoading = false;
                StateHasChanged();
            }

        }
        //Function Update Distributions on change Area select

        private async void UpdateDistributions_FilterByArea()
        {
            ShowLoading = true;
            StateHasChanged();
            try
            {
                if_pick_Distribution = false;

                distributionId = 0;
                _distributions.Clear();


                if (areaId != 0)
                {

                    await Filters();

                    foreach (var item in _assychart)
                    {
                        if (!_distributions.Any(d => d.DistributionId == item.DistributionId))
                        {
                            _distributions.Add(item.Distribution);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                ShowLoading = false;
                StateHasChanged();
            }

        }

        private async void SetDistribution(Distribution element)
        {
            distributionId = element.DistributionId;
            Update_FilterByDistribution();
        }

        private async void SetArea(Area element)
        {
            areaId = element.AreaId;
            UpdateDistributions_FilterByArea();
        }
        private async void Update_FilterByDistribution()
        {
            ShowLoading = true;
            StateHasChanged();
            try
            {
                productId = 0;
                if_pick_Distribution = true;


                if (distributionId != 0)
                {
                    await Filters();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                ShowLoading = false;
                StateHasChanged();
            }

            StateHasChanged();
        }


        private async Task<AsyncVoidMethodBuilder> Filters()
        {
            Console.WriteLine("Entro en filtros");
            if (plantId != 0)
            {
                Console.WriteLine("plant != 0");

                _assychart = _assychartItems.Where(a => a.PlantId == plantId).ToList();
            }

            if (areaId != 0 && plantId != 0)
            {
                Console.WriteLine("P0 A0");

                _assychart = _assychart.Where(a => a.AreaId == areaId).ToList();
            }
            else if (areaId != 0)
            {
                Console.WriteLine("p0 Ax");

                _assychart = _assychartItems.Where(a => a.AreaId == areaId).ToList();
            }

            if (areaId != 0 && distributionId != 0)
            {
                Console.WriteLine("Ax Do");

                _assychart = _assychart.Where(a => a.DistributionId == distributionId).ToList();
            }
            else if (distributionId != 0)
            {
                Console.WriteLine("A0 Dx");

                _assychart = _assychartItems.Where(a => a.DistributionId == distributionId).ToList();
            }

            if (productId != 0)
            {
                Console.WriteLine("Prod X");

                _assychart = _assychart.Where(a => a.RoutesProductsAssyChart?.Any(r => r.ProductId == productId) == true).ToList();
            }

            if (plantId == 0 && areaId == 0 && distributionId == 0 && productId == 0)
            {
                Console.WriteLine("A0");

                _assychart = ObjectCloner.ObjectCloner.DeepClone(_assychartItems);
            }

            if (idFilter != 0)
            {
                _assychart = _assychart.Where(a => a.AssyChardId.ToString().Contains(idFilter.ToString().ToLower(), StringComparison.InvariantCultureIgnoreCase)).ToList();

            }

            return new AsyncVoidMethodBuilder();
        }

        private async Task<IEnumerable<Area>> SearchArea(string value)
        {
            // In real life use an asynchronous function for fetching data from an api.
            // await Task.Delay(1000);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return _areas;

            return _areas.Where(x => x.Code.ToLower().Contains(value.ToLower(), StringComparison.InvariantCultureIgnoreCase) || x.Description.ToLower().Contains(value.ToLower(), StringComparison.InvariantCultureIgnoreCase));
        }

        private async Task<IEnumerable<Distribution>> SearchDistribution(string value)
        {
            // In real life use an asynchronous function for fetching data from an api.
            // await Task.Delay(1000);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return _distributions;

            return _distributions.Where(x => x.Code.ToLower().Contains(value.ToLower(), StringComparison.InvariantCultureIgnoreCase) || x.Description.ToLower().Contains(value.ToLower(), StringComparison.InvariantCultureIgnoreCase));
        }

        //Filtering

        private bool FilterFunc(AssyChart element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;

            try
            {
                if (element.AssyChardId.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            catch (Exception ex)
            {

            }

            if (element.RoutesProductsAssyChart?.Count > 0)
            {
                if (element.RoutesProductsAssyChart.Any(r => r.Product?.Code.ToLower() == searchString.ToLower()))
                    return true;

                if (element.RoutesProductsAssyChart.Any(r => r.Product?.Description.ToLower() == searchString.ToLower()))
                    return true;
            }

            if (element.Plant != null)
            {
                if (element.Plant.Description.ToLower().Contains(searchString.ToLower(), StringComparison.OrdinalIgnoreCase))
                    return true;

                if (element.Plant.Code.ToLower().Contains(searchString.ToLower(), StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            if (element.Area != null)
            {
                if (element.Area.Description.ToLower().Contains(searchString.ToLower(), StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Area.Code.ToLower().Contains(searchString.ToLower(), StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            if (element.Distribution != null)
            {
                if (element.Distribution.Description.ToLower().Contains(searchString.ToLower(), StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Distribution.Code.ToLower().Contains(searchString.ToLower(), StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            //if (element.GOS.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            //    return true;
            //if (element.CCP.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            //    return true;
            //if (element.HOE.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            //    return true;
            if (element.Operation != null)
            {
                if (element.Operation.Description.ToLower().Contains(searchString.ToLower(), StringComparison.OrdinalIgnoreCase))
                    return true;

                if (element.Operation.Code.ToLower().Contains(searchString.ToLower(), StringComparison.OrdinalIgnoreCase))
                    return true;
            }


            //if (element.Product.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            //    return true;

            if ($"{element.Operation?.Description.ToLower()} {element.Plant?.Description.ToLower()} {element.Area?.Description.ToLower()}{element.Distribution?.Description.ToLower()}".Contains(searchString.ToLower()))
                return true;

            return false;
        }


        void GoToUpdateAssyChart(int assychartid)
        {
            NavigationManager.NavigateTo($"assychart/updateassychart/{assychartid}");
        }
        void GoToDetailsAssyChart(int assychartid)
        {
            NavigationManager.NavigateTo($"assychart/details/{assychartid}");
        }

        async Task DeleteAssyChart(int assychartid)
        {

            bool confirm = await JS.InvokeAsync<bool>("confirm", $"{@Localizer["ACquestionDelete"]}");

            if (confirm)
            {
                _assychart.RemoveAll(assychart => assychart.AssyChardId == assychartid);
                await AssyChartServices.DeleteAssyChart(assychartid);
            }



        }

        int SOSCodePathId { get; set; } = 0;


        private async Task<AsyncVoidMethodBuilder> OpenDialogCodePath(SOSCodePath itemselected, MudTabPanel panelSelect)
        {
            ShowLoading = true;
            SOSCodePathId = itemselected.SOSCodePathId;
           

            CodePathModalDisplay = true;
            StateHasChanged();

            ShowLoading = false;

            return new AsyncVoidMethodBuilder();
        }
       


        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true,MaxWidth = MaxWidth.ExtraLarge, FullWidth = true, DisableBackdropClick = true, CloseButton = true  };
        private DialogOptions dialogDeleteOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, Position = DialogPosition.TopCenter, DisableBackdropClick = true, CloseButton = true };


        private async void CloseModalFiles()
        {
            CodePathModalDisplay = false;
            ShowLoading = false;

            StateHasChanged();

        }

        private int selectedRowNumber = -1;
        private MudTable<AssyChart> SelectTableEvent;

        private void RowClickEvent(TableRowClickEventArgs<AssyChart> tableRowClickEventArgs)
        {
        }

        private string SelectedRowClassFunc(AssyChart element, int rowNumber)
        {
            if (selectedRowNumber == rowNumber)
            {
                selectedRowNumber = -1;
                if (SelectTableEvent.SelectedItem != null && SelectTableEvent.SelectedItem.Equals(element))
                {
                    //NavigationManager.NavigateTo($"/assychart/details/{element.AssyChardId}");

                    NavigationManager.NavigateTo($"assychart/details/{element.AssyChardId}");

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
