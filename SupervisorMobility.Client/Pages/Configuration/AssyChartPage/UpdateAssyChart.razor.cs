using Microsoft.JSInterop;
using MudBlazor;
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


        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Assy Chart", href: "/assychart"),
            new BreadcrumbItem("Update Assy Chart", href: "", disabled: true)
        };

        //objects
        AssyChart _assychart = new();
        List<Plant> _plants { get; set; } = new();
        List<Area> _areas = new();
        List<Product> _products = new();
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



        //Inizialize
        protected async override Task OnInitializedAsync()
        {
            _assychart = await AssyChartServices.GetAssyChart(assychartId);

            _plants = await PlantServices.GetPlants();
            _areas = await AreaServices.GetAreas(_assychart.PlantId);
            _distributions = await DistributionServices.GetDistributions(_assychart.PlantId, _assychart.AreaId);
            _products = await ProductServices.GetProducts();

            _distributionValues = await DistributionServices.GetDistributionWithCollections(_assychart.PlantId, _assychart.AreaId, _assychart.DistributionId);


            GOSFolders = await CDMSServices.GetFoldersGOS();
            if (GOSFolders != null)
            {
                folderGOSError = false;
                rootNodeGOS = ConstruirArbolGOS(GOSFolders.operation);
            }
            else
            {
                folderGOSError = true;
            }

            CCPFolders = await CDMSServices.GetFoldersCCP();
            if (CCPFolders != null)
            {
                folderCCPError = false;
                rootNodeCCP = ConstruirArbolCCP(CCPFolders.operation);
            }
            else
            {
                folderCCPError = true;
            }


            HOEFolders = await CDMSServices.GetFoldersHOE();
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
            _areas = await AreaServices.GetAreas(_assychart.PlantId);
        }

        private async void UpdateDistributions()
        {
            _distributions = await DistributionServices.GetDistributions(_assychart.PlantId, _assychart.AreaId);
        }

        private async void UpdateOperationProducts()
        {
            _distributionValues = await DistributionServices.GetDistributionWithCollections(_assychart.PlantId, _assychart.AreaId, _assychart.DistributionId);
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

        async void UpdateAssyChartAsync()
        {
            GosFilesInFolder = new CDMS_GOS_Archives();

            if (_assychart.GOS != "")
            {
                GosFilesInFolder = await CDMSServices.GetFilesGOS(_assychart.GOS);
                if (GosFilesInFolder.message == "NO FILES IN DIRECTORY")
                {
                    isGosFolder = false;
                    bool msgGOSBox = await OpenMessageGOS();

                }
                else
                {
                    isGosFolder = true;
                }
            }
            else { isGosFolder = true; }

            CcpFilesInFolder = new CDMS_CCP_Archives();

            if (_assychart.CCP != "")
            {
                CcpFilesInFolder = await CDMSServices.GetFilesCCP(_assychart.CCP);
                if (GosFilesInFolder.message == "NO FILES IN DIRECTORY")
                {
                    isCcpFolder = false;
                    bool msgCCPBox = await OpenMessageCCP();
                }
                else
                {
                    isCcpFolder = true;
                }
            }
            else
            {
                isCcpFolder = true;

            }

            HoeFilesInFolder = new CDMS_HOE_Archives();

            if (_assychart.HOE != "")
            {
                HoeFilesInFolder = await CDMSServices.GetFilesHOE(_assychart.HOE);
                if (HoeFilesInFolder.message == "NO FILES IN DIRECTORY")
                {
                    isHoeFolder = false;
                    bool msgHOEBox = await OpenMessageHOE();

                }
                else
                {
                    isHoeFolder = true;
                }
            }
            else
            {
                isHoeFolder = true;
            }



            if (isGosFolder && isCcpFolder && isHoeFolder)
            {
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

            
        }

        void CancelCreateAssyChart()
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
