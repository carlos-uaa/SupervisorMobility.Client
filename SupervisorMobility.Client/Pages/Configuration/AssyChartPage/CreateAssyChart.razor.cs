using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities.TreeStruct;
using SupervisorMobility.Client.Services.UserService;


namespace SupervisorMobility.Client.Pages.Configuration.AssyChartPage
{
    public partial class CreateAssyChart
    {
        //objects
        AssyChart _newassychart = new();
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
        public bool enablecreate { get; set; } = false;
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
        protected override async void OnInitialized()
        {
            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                new BreadcrumbItem(text: Localizer["assychart"], href: "/assychart"),
                new BreadcrumbItem(text: Localizer["ACNewAC"], href: "", disabled: true),
            };
            _plants = await PlantServices.GetPlants();
            _products = await ProductServices.GetProducts();

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



            StateHasChanged();
        }



        //Function Update Area on change plant select

        async void UpdateAreas()
        {
            _areas = await AreaServices.GetAreas(auxplant);
            auxarea = 0;
            auxdistribution = 0;
            _newassychart.RoutesProductsAssyChart?.Clear();
            ProductSelected = new();
            _distributionValues = new();
            StateHasChanged();

        }
        //Function Update Distributions on change Area select

        private async void UpdateDistributions()
        {
            auxdistribution = 0;
            _newassychart.RoutesProductsAssyChart?.Clear();
            _distributions = await DistributionServices.GetDistributionsWithCollections(auxplant, auxarea);
            ProductSelected = new();
            _distributionValues = new();
            StateHasChanged();
        }

        private async void UpdateOperationProducts()
        {
            auxoperation = 0;
            _newassychart.RoutesProductsAssyChart?.Clear();
            ProductSelected = new();
            _distributionValues = await DistributionServices.GetDistributionWithCollections(auxplant, auxarea, auxdistribution);


            StateHasChanged();
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

        private void OpenDialogProduct(SOSCodePath itemselected)
        {
            CodePathDialog = itemselected;

            isHoeFolder = false;
            isGosFolder = false;
            isCcpFolder = false;


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


     
        void DeleteGOSRoute(RouteProductAssyChart item)
        {
            item.GOS = "";
        }
        void DeleteCCPRoute(RouteProductAssyChart item)
        {
            item.CCP = "";
        }
        void DeleteHOERoute(RouteProductAssyChart item)
        {
            item.HOE = "";
        }

        async void CreateNewAssyChartAsync()
        {
            enablecreate = true;
            StateHasChanged();
            _newassychart.IsActive = true;
            _newassychart.PlantId = auxplant;
            _newassychart.AreaId = auxarea;
            _newassychart.DistributionId = auxdistribution;
            _newassychart.OperationId = auxoperation;

            _newassychart.CreationDate = DateTime.Now;

            if (auxErgonomicsLevel != 0)
            {
                _assychart.ErgonomicsLevel = auxErgonomicsLevel;
            }

            var anyAssyChart = await AssyChartServices.GetAssyChartJobObservation(auxplant, auxarea, auxdistribution);

            if(anyAssyChart != null)
            {
                var result = await DialogService.ShowMessageBox(
                      $"{Localizer["AC_Msg_Title"]}",
                      (MarkupString)$"{Localizer["AC_Msg_Body"]}",
                      yesText: $"{Localizer["AC_Msg_Continue"]}!", cancelText: $"{Localizer["AC_Msg_BackEdit"]}!");


                switch (result)
                {
                    case null:
                        //continuar con la edicion
                        enablecreate = false;

                        break;
                    case true:
                        //continuar sin reasignar nada
                        var CreateAssychartResult = await AssyChartServices.CreateAssyChart(_newassychart);

                        if (CreateAssychartResult != null)
                            NavigationManager.NavigateTo("/assychart");
                        else
                            await JsRuntime.InvokeVoidAsync("alert", "Fail to create Assy Chart, contact admin!"); // Alert
                        break;

                    case false:
                        break;
                }

            }
            else
            {
                var result = await AssyChartServices.CreateAssyChart(_newassychart);

                if (result != null)
                    NavigationManager.NavigateTo("/assychart");
                else
                    await JsRuntime.InvokeVoidAsync("alert", "Fail to create Assy Chart, contact admin!"); // Alert
            }



            enablecreate = false;
            StateHasChanged();
        }

        void CancelCreateAssyChart()
        {
            NavigationManager.NavigateTo("/assychart");
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



        public async Task<HashSet<TreeItemData>> LoadServerData(TreeItemData parentNode)
        {
            await Task.Delay(50);
            return parentNode.TreeItems;
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

        public void CreateCodePath()
        {
            if (_assychart.RoutesProductsAssyChart == null)
            {
                _assychart.RoutesProductsAssyChart = new List<SOSCodePath>();
            }


            SOSCodePath _newCodePathAssyChart = new();
            _newCodePathAssyChart.AssyChardId = _assychart.AssyChardId;
            _newCodePathAssyChart.DistributionId = auxdistribution;

            _newCodePathAssyChart.IsActive = true;

            _assychart.RoutesProductsAssyChart.Add(_newCodePathAssyChart);


            CodePathDialog = _newCodePathAssyChart;

            IndexProd = -1;
            _product = new();


            CreatePathCodeModalDisplay = true;
            StateHasChanged();
        }//create code open dialog
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
