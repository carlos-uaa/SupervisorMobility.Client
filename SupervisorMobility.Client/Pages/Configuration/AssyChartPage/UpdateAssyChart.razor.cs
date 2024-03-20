using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.TreeStruct;
using SupervisorMobility.Client.Pages.Configuration.ProductPage;
using SupervisorMobility.Client.Services.ProductsService;
using static SupervisorMobility.Client.Pages.Configuration.AssyChartPage.CreateAssyChart;

namespace SupervisorMobility.Client.Pages.Configuration.AssyChartPage
{
    public partial class UpdateAssyChart
    {

        [Parameter]
        public int assychartId { get; set; }

        TreeItemData rootNodeCCP { get; set; } = new TreeItemData();
        TreeItemData rootNodeGOS { get; set; } = new TreeItemData();
        TreeItemData rootNodeHOE { get; set; } = new TreeItemData();
        TreeItemData SelectedNodeCCP { get; set; }
        TreeItemData SelectedNodeGOS { get; set; }
        TreeItemData SelectedNodeHOE { get; set; }

        CDMS_GOS_Directory GOSFolders { get; set; } = new CDMS_GOS_Directory();
        CDMS_CCP_Directory CCPFolders { get; set; } = new CDMS_CCP_Directory();
        CDMS_HOE_Directory HOEFolders { get; set; } = new CDMS_HOE_Directory();

        private bool folderCCPError = false;
        private bool folderHOEError = false;
        private bool folderGOSError = false;
        Distribution _distributionValues = new();


        private List<BreadcrumbItem> _links;

        //objects
        

        bool isGosFolder = false;
        bool isCcpFolder = false;
        bool isHoeFolder = false;
        private CDMS_CCP_Archives? CcpFilesInFolder;
        private CDMS_HOE_Archives? HoeFilesInFolder;
        private CDMS_GOS_Archives? GosFilesInFolder;
        MudMessageBox HOEmbox { get; set; }
        MudMessageBox CCPmbox { get; set; }
        MudMessageBox GOSmbox { get; set; }
        MudMessageBox CreateCodePathErrormbox { get; set; }

        public bool DisplayLoading { get; set; } = true;
        public bool EnableUpdate { get; set; } = false;
        public bool modeDisplay { get; set; } = false;
        public bool ProductModalDisplay { get; set; } = false;
        public bool CreatePathCodeModalDisplay { get; set; } = false;


        Product? ProductSelected { get; set; } = null;
        SOSCodePath CodePathDialog { get; set; }
        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraExtraLarge, FullWidth = true, Position = DialogPosition.TopCenter, DisableBackdropClick = true, CloseButton = true };
        private DialogOptions dialogOptionsOutClose = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraExtraLarge, FullWidth = true, Position = DialogPosition.TopCenter, DisableBackdropClick = true, CloseButton = false };

        [Inject] private IDialogService DialogService { get; set; }

        AssyChart _assychart = new();
        List<Plant> _plants { get; set; } = new();
        List<Area> _areas = new();
        List<Distribution> _distributions { get; set; } = new();
        private int auxplant;
        private int auxarea;
        private int auxdistribution;
        private int auxoperation;
        private Product _product = new Product();
        int IndexProd = -1;
        private List<Product> _products = new List<Product>();

        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Dark };

        bool if_add_CD_CCP = false;
        bool if_add_CD_GOS = false;
        bool if_add_CD_HOE = false;


        public int auxErgonomicsLevel = 0;

        //Inizialize
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
            try
            {



                _links = new List<BreadcrumbItem>
              {
                    new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                new BreadcrumbItem(text: Localizer["assychart"], href: "/assychart"),
                new BreadcrumbItem(text: Localizer1["ACUpdate"], href: "", disabled: true),
              };
                _assychart = await AssyChartServices.GetAssyChart(assychartId);

                if(_assychart.ErgonomicsLevel != null)
                {
                    auxErgonomicsLevel = (int)_assychart.ErgonomicsLevel;
                }

                _products = await ProductsServices.GetProducts();


                auxplant = _assychart.PlantId != null ? (int)_assychart.PlantId : 0;
                auxarea = _assychart.AreaId != null ? (int)_assychart.AreaId : 0;
                auxdistribution = _assychart.DistributionId != null ? (int)_assychart.DistributionId : 0;
                auxoperation = _assychart.OperationId != null ? (int)_assychart.OperationId : 0;

                try
                {
                    _plants = await PlantServices.GetPlants();
                }
                catch (Exception exe)
                {
                    Console.WriteLine("Error Get Plants");
                    if (_assychart.PlantId == 0)
                    {
                        Console.WriteLine("Plant id is null");
                    }
                    Console.WriteLine(exe.Message);
                }

                try
                {
                    _areas = await AreaServices.GetAreas(auxplant);
                }
                catch (Exception exe)
                {
                    Console.WriteLine("Error Get Areas");

                    if (_assychart.PlantId == 0)
                    {
                        Console.WriteLine("Plant id is null");
                    }

                    if (_assychart.AreaId == 0)
                    {
                        Console.WriteLine("Area id is null");
                    }
                    Console.WriteLine(exe.Message);
                }

                try
                {
                    _distributions = await DistributionServices.GetDistributions(auxplant, auxarea);
                    _distributionValues = await DistributionServices.GetDistributionWithCollections(auxplant, auxarea, auxdistribution);

                }
                catch (Exception exe)
                {
                    Console.WriteLine("Error Get Distribution");
                    if (_assychart.PlantId == 0)
                    {
                        Console.WriteLine("Plant id is null");
                    }

                    if (_assychart.AreaId == 0)
                    {
                        Console.WriteLine("Area id is null");
                    }
                    Console.WriteLine(exe.Message);
                }




                try
                {
                    if (_distributionValues != null)
                    {
                        foreach (var item in _assychart.RoutesProductsAssyChart)
                        {
                            if (_distributionValues.Products.Any(p => p.ProductId == item.ProductId) == true)
                            {
                                _distributionValues.Products.RemoveAll(p => p.ProductId == item.ProductId);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error remove duplicates");
                    Console.WriteLine(ex.Message);
                }


                try
                {
                    GOSFolders = await CDMSServices.GetFoldersGOS();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Get GOS Folder From CDMS");
                    Console.WriteLine(ex.Message);
                }

                if (GOSFolders != null)
                {
                    folderGOSError = false;
                    rootNodeGOS = TreeServices.Make_Tree_GOS(GOSFolders.operation);
                }
                else
                {
                    folderGOSError = true;
                }

                try
                {
                    CCPFolders = await CDMSServices.GetFoldersCCP();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Get CCP Folder From CCP");
                    Console.WriteLine(ex.Message);
                }

                if (CCPFolders != null)
                {
                    folderCCPError = false;
                    rootNodeCCP = TreeServices.Make_Tree_CCP(CCPFolders.operation);
                }
                else
                {
                    folderCCPError = true;
                }

                try
                {
                    HOEFolders = await CDMSServices.GetFoldersHOE();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Get HOE Folder From CDMS");
                    Console.WriteLine(ex.Message);
                }
                if (HOEFolders != null)
                {
                    folderHOEError = false;
                    rootNodeHOE = TreeServices.Make_Tree_HOE(HOEFolders.operation);


                }
                else
                {
                    folderHOEError = true;
                }

            }
            catch (Exception ex)
            {

            }
            finally
            {
                DisplayLoading = false;
            }

            StateHasChanged();

        }



        //Function Update Selectd

        async void UpdateAreas()
        {
            auxdistribution = 0;
            auxoperation = 0;
            ProductSelected = new();
            _distributionValues = new();
            _areas = await AreaServices.GetAreas(auxplant);
            StateHasChanged();
        }

        private async void UpdateDistributions()
        {
            auxoperation = 0;
            ProductSelected = new();
            _distributionValues = new();
            _distributions = await DistributionServices.GetDistributions(auxplant, auxarea);
            StateHasChanged();
        }

        private async void UpdateOperationProducts()
        {
            ProductSelected = new();
            _distributionValues = await DistributionServices.GetDistributionWithCollections(auxplant, auxarea, auxdistribution);
            StateHasChanged();
        }
        private async Task<bool> OpenMessageHOE()
        {
            bool? result = await HOEmbox.Show();

            return result == null ? false : true;
        }
        private async Task<bool> OpenMessageCCP()
        {
            bool? result = await CCPmbox.Show();

            return result == null ? false : true;
        }
        private async Task<bool> OpenMessageGOS()
        {
            bool? result = await GOSmbox.Show();

            return result == null ? false : true;
        }

        private async Task<IEnumerable<Product>> SearchProduct(string value)
        {
            // In real life use an asynchronous function for fetching data from an api.
            // await Task.Delay(1000);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return _distributionValues.Products;

            return _distributionValues.Products.Where(x => x.Code.Contains(value, StringComparison.InvariantCultureIgnoreCase) || x.Description.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        private async Task SetProductChange(Product ItemSelected)
        {
            if (ProductSelected == null)
                ProductSelected = new();


            ProductSelected = ItemSelected;

            StateHasChanged();
        }

        private void OpenDialogEditCodePath(SOSCodePath itemselected)
        {
            CodePathDialog = itemselected;

            isHoeFolder = false;
            isGosFolder = false;
            isCcpFolder = false;


            if_add_CD_CCP = CodePathDialog.CommonDirectionCCP != "";
            if_add_CD_GOS = CodePathDialog.CommonDirectionGOS != "";
            if_add_CD_HOE = CodePathDialog.CommonDirectionHOE != "";


            ProductModalDisplay = true;
            StateHasChanged();

        }
        private async void CloseProductModal()
        {
            GosFilesInFolder = new CDMS_GOS_Archives();

            if (CodePathDialog.GOS != "")
            {
                try
                {
                    GosFilesInFolder = await CDMSServices.GetFilesGOS(CodePathDialog.GOS);
                    if (GosFilesInFolder.message == "NO FILES IN DIRECTORY" || GosFilesInFolder.message == "NO FILES OR DIRECTORIES")
                    {
                        isGosFolder = false;
                        bool msgGOSBox = await OpenMessageGOS();

                    }
                    else
                    {
                        isGosFolder = true;
                    }
                }
                catch (Exception ex)
                {
                    isGosFolder = false;
                    bool msgGOSBox = await OpenMessageGOS();
                    Console.WriteLine(ex.ToString());
                }


            }
            else { isGosFolder = true; }

            CcpFilesInFolder = new CDMS_CCP_Archives();

            if (CodePathDialog.CCP != "")
            {
                try
                {
                    CcpFilesInFolder = await CDMSServices.GetFilesCCP(CodePathDialog.CCP);
                    if (CcpFilesInFolder.message == "NO FILES IN DIRECTORY" || CcpFilesInFolder.message == "NO FILES OR DIRECTORIES")
                    {
                        isCcpFolder = false;
                        bool msgCCPBox = await OpenMessageCCP();
                    }
                    else
                    {
                        isCcpFolder = true;
                    }

                }
                catch (Exception ex)
                {
                    isCcpFolder = false;
                    bool msgCCPBox = await OpenMessageCCP();
                    Console.WriteLine(ex.ToString());
                }


            }
            else
            {
                isCcpFolder = true;

            }

            HoeFilesInFolder = new CDMS_HOE_Archives();




            if (CodePathDialog.HOE != "")
            {
                try
                {
                    HoeFilesInFolder = await CDMSServices.GetFilesHOE(CodePathDialog.HOE);

                    if (HoeFilesInFolder.message == "NO FILES IN DIRECTORY" || HoeFilesInFolder.message == "INCOMPLETE FIELDS FOR HOE in ⪢ ⪢ ⪢ ⪢ VALIDATE_PAD_HOE" || HoeFilesInFolder.message == "NO FILES OR DIRECTORIES")
                    {
                        isHoeFolder = false;
                        bool msgHOEBox = await OpenMessageHOE();

                    }
                    else
                    {
                        isHoeFolder = true;
                    }
                }
                catch (Exception ex)
                {
                    isHoeFolder = false;
                    bool msgHOEBox = await OpenMessageHOE();
                    Console.WriteLine(ex.ToString());
                }

            }
            else
            {
                isHoeFolder = true;
            }


            if (isGosFolder && isCcpFolder && isHoeFolder)
            {
                ProductModalDisplay = false;
            }

            StateHasChanged();
        }


 

        void DeleteProductToList(SOSCodePath item)
        {
            _distributionValues.Products.Add(item.Product);
            _assychart.RoutesProductsAssyChart?.Remove(item);
            StateHasChanged();

        }

        void DeleteGOSRoute()
        {
            CodePathDialog.GOS = "";
        }
        void DeleteCCPRoute()
        {
            CodePathDialog.CCP = "";
        }
        void DeleteHOERoute()
        {
            CodePathDialog.HOE = "";
        }
        void DeleteGOSRouteCD()
        {
            CodePathDialog.CommonDirectionGOS = "";
        }
        void DeleteCCPRouteCD()
        {
            CodePathDialog.CommonDirectionCCP = "";
        }
        void DeleteHOERouteCD()
        {
            CodePathDialog.CommonDirectionHOE = "";
        }

        private void AddRemove_CD_HOE()
        {
            if_add_CD_HOE = !if_add_CD_HOE;
            StateHasChanged();
        }
        private void AddRemove_CD_GOS()
        {
            if_add_CD_GOS = !if_add_CD_GOS;
            StateHasChanged();
        }
        private void AddRemove_CD_CCP()
        {
            if_add_CD_CCP = !if_add_CD_CCP;
            StateHasChanged();
        }
        async void UpdateAssyChartAsync()
        {
            EnableUpdate = true;

            _assychart.IsActive = true;
            _assychart.PlantId = auxplant;
            _assychart.AreaId = auxarea;
            _assychart.DistributionId = auxdistribution;
            _assychart.OperationId = auxoperation;

            _assychart.ModificationDate = DateTime.Now;

            if (auxErgonomicsLevel != 0)
            {
                _assychart.ErgonomicsLevel = auxErgonomicsLevel;
            }

            var anyAssyChart = await AssyChartServices.GetAssyChartJobObservation(auxplant, auxarea, auxdistribution);

            if (anyAssyChart != null && anyAssyChart.AssyChardId != _assychart.AssyChardId)
            {
                var result = await DialogService.ShowMessageBox(
                      $"{Localizer["AC_Msg_Title"]}",
                      (MarkupString)$"{Localizer["AC_Msg_Body"]}",
                      yesText: $"{Localizer["AC_Msg_Continue"]}!", cancelText: $"{Localizer["AC_Msg_BackEdit"]}!");


                switch (result)
                {
                    case null:
                        //continuar con la edicion
                        EnableUpdate = false;


                        break;
                    case true:
                        //continuar sin reasignar nada
                        var UpadteAssychart = await AssyChartServices.UpdateAssyChart(assychartId, _assychart);


                        if (UpadteAssychart)
                        {
                            await JsRuntime.InvokeVoidAsync("alert", "Succesful Update!"); // Alert
                            NavigationManager.NavigateTo("/assychart");
                        }
                        else
                            await JsRuntime.InvokeVoidAsync("alert", "Fallo Actualizacion!"); // Alert

                        break;

                    case false:
                        break;
                }

            }
            else
            {
                var result = await AssyChartServices.UpdateAssyChart(assychartId, _assychart);


                if (result)
                {
                    await JsRuntime.InvokeVoidAsync("alert", "Succesful Update!"); // Alert
                    NavigationManager.NavigateTo("/assychart");
                }
                else
                    await JsRuntime.InvokeVoidAsync("alert", "Fallo Actualizacion!"); // Alert

            }


            EnableUpdate = false;
            StateHasChanged();

        }



        void CancelUpdateAssyChart()
        {
            NavigationManager.NavigateTo("/assychart");
        }
        public async Task<HashSet<TreeItemData>> LoadServerData(TreeItemData parentNode)
        {
            await Task.Delay(50);
            return parentNode.TreeItems;
        }

        public void CreateCodePath()
        {
            if (_assychart.RoutesProductsAssyChart == null)
            {
                _assychart.RoutesProductsAssyChart = new List<SOSCodePath>();
            }


            SOSCodePath _newCodePathAssyChart = new();
            _newCodePathAssyChart.AssyChardId = _assychart.AssyChardId;
            _newCodePathAssyChart.DistributionId = (int)_assychart.DistributionId;

            _newCodePathAssyChart.IsActive = true;

            _assychart.RoutesProductsAssyChart.Add(_newCodePathAssyChart);


            CodePathDialog = _newCodePathAssyChart;

            IndexProd = -1;
            _product = new();


            CreatePathCodeModalDisplay = true;
            StateHasChanged();
        }//create code open dialog


        void UpdateProduct()
        {
            _product = _products[IndexProd];

            CodePathDialog.Product = _product;
            CodePathDialog.ProductId = _product.ProductId;

            StateHasChanged();
        }
        async void CloseCreateCodePath()
        {
            CreatePathCodeModalDisplay = !(IndexProd != -1 && CodePathDialog.Code != "");


            if (CreatePathCodeModalDisplay)
            {
                bool? result = await CreateCodePathErrormbox.Show();
            }
            StateHasChanged();
        }


    }
}
