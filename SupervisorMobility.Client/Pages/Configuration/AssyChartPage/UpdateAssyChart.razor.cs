using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
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
        AssyChart _assychart = new();
        List<Plant> _plants { get; set; } = new();
        List<Area> _areas = new();
        List<Distribution> _distributions { get; set; } = new();

        bool isGosFolder = false;
        bool isCcpFolder = false;
        bool isHoeFolder = false;
        private CDMS_CCP_Archives? CcpFilesInFolder;
        private CDMS_HOE_Archives? HoeFilesInFolder;
        private CDMS_GOS_Archives? GosFilesInFolder;
        MudMessageBox HOEmbox { get; set; }
        MudMessageBox CCPmbox { get; set; }
        MudMessageBox GOSmbox { get; set; }

        public bool EnableUpdate { get; set; } = false;
        public bool modeDisplay { get; set; } = false;
        public bool ProductModalDisplay { get; set; } = false;


        Product? ProductSelected { get; set; } = null;
        RouteProductAssyChart RouteProductDialogDisplay { get; set; }
        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraExtraLarge, FullWidth = true, Position = DialogPosition.TopCenter, DisableBackdropClick = true, CloseButton = true };



        private int auxplant;
        private int auxarea;
        private int auxdistribution;
        private int auxoperation;

        //Inizialize
        protected async override Task OnInitializedAsync()
        {
            _links = new List<BreadcrumbItem>
              {
                    new BreadcrumbItem(text: Localizer["home"], href: "#"),
                new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                new BreadcrumbItem(text: Localizer["assychart"], href: "/assychart"),
                new BreadcrumbItem(text: Localizer["ACUpdateAC"], href: "", disabled: true),
              };
            _assychart = await AssyChartServices.GetAssyChart(assychartId);

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
                rootNodeGOS = ConstruirArbolGOS(GOSFolders.operation);
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
                rootNodeCCP = ConstruirArbolCCP(CCPFolders.operation);
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
                rootNodeHOE = ConstruirArbolHOE(HOEFolders.operation);


            }
            else
            {
                folderHOEError = true;
            }



            StateHasChanged();

        }



        //Function Update Selectd

        async void UpdateAreas()
        {
            auxdistribution = 0;
            auxoperation = 0;
            _areas = await AreaServices.GetAreas(auxplant);
        }

        private async void UpdateDistributions()
        {
            auxoperation = 0;
            _distributions = await DistributionServices.GetDistributions(auxplant, auxarea);
        }

        private async void UpdateOperationProducts()
        {
            _distributionValues = await DistributionServices.GetDistributionWithCollections(auxplant, auxarea, auxdistribution);
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

        private void OpenDialogProduct(RouteProductAssyChart itemselected)
        {
            RouteProductDialogDisplay = itemselected;

            isHoeFolder = false;
            isGosFolder = false;
            isCcpFolder = false;


            ProductModalDisplay = true;
            StateHasChanged();

        }
        private async void CloseProductModal()
        {
            GosFilesInFolder = new CDMS_GOS_Archives();

            if (RouteProductDialogDisplay.GOS != "")
            {
                try
                {
                    GosFilesInFolder = await CDMSServices.GetFilesGOS(RouteProductDialogDisplay.GOS);
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

            if (RouteProductDialogDisplay.CCP != "")
            {
                try
                {
                    CcpFilesInFolder = await CDMSServices.GetFilesCCP(RouteProductDialogDisplay.CCP);
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




            if (RouteProductDialogDisplay.HOE != "")
            {
                try
                {
                    HoeFilesInFolder = await CDMSServices.GetFilesHOE(RouteProductDialogDisplay.HOE);

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


        private void AddProductToList(Product selection)
        {
            if (_assychart.RoutesProductsAssyChart == null)
            {
                _assychart.RoutesProductsAssyChart = new List<RouteProductAssyChart>();
            }


            if (ProductSelected != null && !_assychart.RoutesProductsAssyChart.Any(a => a.ProductId == selection.ProductId))
            {

                var product = ObjectCloner.ObjectCloner.DeepClone<Product>(ProductSelected);

                RouteProductAssyChart routeProductAssyChart = new();
                routeProductAssyChart.Product = product;
                routeProductAssyChart.ProductId = product.ProductId;
                routeProductAssyChart.IsActive = true;

                _assychart.RoutesProductsAssyChart.Add(routeProductAssyChart);

                _distributionValues.Products.Remove(selection);

                ProductSelected = null;
            }
            StateHasChanged();
        }

        void DeleteProductToList(RouteProductAssyChart item)
        {
            _distributionValues.Products.Add(item.Product);
            _assychart.RoutesProductsAssyChart?.Remove(item);
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
        async void UpdateAssyChartAsync()
        {
            EnableUpdate = true;

            _assychart.IsActive = true;
            _assychart.PlantId = auxplant;
            _assychart.AreaId = auxarea;
            _assychart.DistributionId = auxdistribution;
            _assychart.OperationId = auxoperation;

            _assychart.ModificationDate = DateTime.Now;
            var result = await AssyChartServices.UpdateAssyChart(assychartId, _assychart);


            if (result)
            {
                await JsRuntime.InvokeVoidAsync("alert", "Succesful Update!"); // Alert
                NavigationManager.NavigateTo("/assychart");
            }
            else
                await JsRuntime.InvokeVoidAsync("alert", "Fallo Actualizacion!"); // Alert



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

        public class TreeItemData
        {
            public string Nombre { get; set; }
            public string Ruta { get; set; }
            public bool EsDirectorio { get; set; }
            public HashSet<TreeItemData> TreeItems { get; set; } = new HashSet<TreeItemData>();

            public TreeItemData()
            {
                TreeItems = new HashSet<TreeItemData>();
            }


            public TreeItemData(string nombre, string ruta, bool esDirectorio)
            {
                Nombre = nombre;
                Ruta = ruta;
                EsDirectorio = esDirectorio;
                TreeItems = new HashSet<TreeItemData>();
            }


            public void AgregarNodo(TreeItemData nodo)
            {
                TreeItems.Add(nodo);
            }

        }

        public TreeItemData ConstruirArbolCCP(List<FolderCCP> elementos)
        {
            TreeItemData root = new TreeItemData { Nombre = "Raíz", Ruta = "", EsDirectorio = true };
            root.TreeItems = new HashSet<TreeItemData>();

            foreach (var itemData in elementos)
            {
                // Dividir la ruta en partes y crear cada nodo del árbol
                string[] rutaPartes = itemData.ruta.Split('/');
                TreeItemData parent = root;

                for (int i = 0; i < rutaPartes.Length; i++)
                {
                    string nombre = rutaPartes[i];

                    // Buscar el nodo en los hijos del padre actual
                    TreeItemData nodoActual = parent.TreeItems.ToList().Find(child => child.Nombre == nombre);

                    if (nodoActual == null)
                    {
                        // Si el nodo no existe, crearlo y agregarlo como hijo del padre actual
                        nodoActual = new TreeItemData() { Nombre = nombre, Ruta = itemData.ruta, EsDirectorio = true };
                        nodoActual.TreeItems = new HashSet<TreeItemData>();
                        parent.TreeItems.Add(nodoActual);
                    }

                    // Actualizar el padre actual
                    parent = nodoActual;
                }

                // Agregar el nodo final (hoja)
                //TreeItemData hoja = new TreeItemData { Nombre = itemData.Nombre, Ruta = itemData.Ruta, EsDirectorio = true };
                //hoja.TreeItems = null;
                //parent.TreeItems.Add(hoja);
            }

            // Imprimir el árbol
            return root;
        }
        public TreeItemData ConstruirArbolHOE(List<FolderHOE> elementos)
        {
            TreeItemData root = new TreeItemData { Nombre = "Raíz", Ruta = "", EsDirectorio = true };
            root.TreeItems = new HashSet<TreeItemData>();

            foreach (var itemData in elementos)
            {
                // Dividir la ruta en partes y crear cada nodo del árbol
                string[] rutaPartes = itemData.ruta.Split('/');
                TreeItemData parent = root;

                for (int i = 0; i < rutaPartes.Length; i++)
                {
                    string nombre = rutaPartes[i];

                    // Buscar el nodo en los hijos del padre actual
                    TreeItemData nodoActual = parent.TreeItems.ToList().Find(child => child.Nombre == nombre);

                    if (nodoActual == null)
                    {
                        // Si el nodo no existe, crearlo y agregarlo como hijo del padre actual
                        nodoActual = new TreeItemData() { Nombre = nombre, Ruta = itemData.ruta, EsDirectorio = true };
                        nodoActual.TreeItems = new HashSet<TreeItemData>();
                        parent.TreeItems.Add(nodoActual);
                    }

                    // Actualizar el padre actual
                    parent = nodoActual;
                }

                // Agregar el nodo final (hoja)
                //TreeItemData hoja = new TreeItemData { Nombre = itemData.Nombre, Ruta = itemData.Ruta, EsDirectorio = true };
                //hoja.TreeItems = null;
                //parent.TreeItems.Add(hoja);
            }

            // Imprimir el árbol
            return root;
        }
        public TreeItemData ConstruirArbolGOS(List<FolderGOS> elementos)
        {
            TreeItemData root = new TreeItemData { Nombre = "Raíz", Ruta = "", EsDirectorio = true };
            root.TreeItems = new HashSet<TreeItemData>();

            foreach (var itemData in elementos)
            {
                // Dividir la ruta en partes y crear cada nodo del árbol
                string[] rutaPartes = itemData.ruta.Split('/');
                TreeItemData parent = root;

                for (int i = 0; i < rutaPartes.Length; i++)
                {
                    string nombre = rutaPartes[i];

                    // Buscar el nodo en los hijos del padre actual
                    TreeItemData nodoActual = parent.TreeItems.ToList().Find(child => child.Nombre == nombre);

                    if (nodoActual == null)
                    {
                        // Si el nodo no existe, crearlo y agregarlo como hijo del padre actual
                        nodoActual = new TreeItemData() { Nombre = nombre, Ruta = itemData.ruta, EsDirectorio = true };
                        nodoActual.TreeItems = new HashSet<TreeItemData>();
                        parent.TreeItems.Add(nodoActual);
                    }

                    // Actualizar el padre actual
                    parent = nodoActual;
                }

                // Agregar el nodo final (hoja)
                //TreeItemData hoja = new TreeItemData { Nombre = itemData.Nombre, Ruta = itemData.Ruta, EsDirectorio = true };
                //hoja.TreeItems = null;
                //parent.TreeItems.Add(hoja);
            }

            // Imprimir el árbol
            return root;
        }

    }
}
