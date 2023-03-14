using Microsoft.JSInterop;
using MudBlazor;


namespace SupervisorMobility.Client.Pages.Configuration.AssyChartPage
{
    public partial class CreateAssyChart
    {
        //objects
        AssyChart _newassychart = new();
        List<Plant> _plants { get; set; } = new();
        List<Area> _areas = new();
        List<Product> _products = new();
        List<Distribution> _distributions { get; set; } = new();


        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Assy Chart", href: "/assychart"),
            new BreadcrumbItem("New Assy Chart", href: "", disabled: true),
        };

        //Inizialize
        protected async override Task OnInitializedAsync()
        {
            _plants = await PlantServices.GetPlants();
            _products = await ProductServices.GetProducts();
        }



        //Function Update Area on change plant select

        async void UpdateAreas()
        {
            _areas = await AreaServices.GetAreas(_newassychart.PlantId);
        }
        //Function Update Distributions on change Area select

        private async void UpdateDistributions()
        {
            _distributions = await DistributionServices.GetDistributions(_newassychart.PlantId, _newassychart.AreaId);
        }

        async void CreateNewAssyChartAsync()
        {
            //_newassychart.CreationDate = DateTime.Now;
            var result = await AssyChartServices.CreateAssyChart(_newassychart);
            if(result != null)
                NavigationManager.NavigateTo("/assychart");
            else
                await JsRuntime.InvokeVoidAsync("alert", "Error en los datos!"); // Alert
        }

        void CancelCreateAssyChart()
        {
            NavigationManager.NavigateTo("/assychart");
        }

    
        TreeItemData rootNodeCCP { get; set; }
        TreeItemData rootNodeGOS { get; set; } = new TreeItemData();
        TreeItemData rootNodeHOE { get; set; }
        TreeItemData SelectedNodeCCP { get; set; }
        TreeItemData SelectedNodeGOS { get; set; }
        TreeItemData SelectedNodeHOE { get; set; }

        FoldersCDMS GOSFolders { get ; set; } = new FoldersCDMS();
        FoldersCDMS CCPFolders { get ; set; } = new FoldersCDMS();
        FoldersCDMS HOEFolders { get ; set; } = new FoldersCDMS();



        protected override async void OnInitialized()
        {
            GOSFolders = await CDMSServices.GetFoldersGOS();
            CCPFolders = await CDMSServices.GetFoldersCCP();
            HOEFolders = await CDMSServices.GetFoldersHOE();
            rootNodeCCP = ConstruirArbol(CCPFolders.operation);
            rootNodeHOE = ConstruirArbol(HOEFolders.operation);
            rootNodeGOS = ConstruirArbol(GOSFolders.operation);
            StateHasChanged();
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


        public async Task<HashSet<TreeItemData>> LoadServerData(TreeItemData parentNode)
        {
            await Task.Delay(500);
            return parentNode.TreeItems;
        } 
        public async Task<HashSet<TreeItemData>> LoadServerData2(TreeItemData parentNode)
        {
            await Task.Delay(500);
            return parentNode.TreeItems;
        }


        public TreeItemData ConstruirArbol(List<OperationFolders> elementos)
        {
            TreeItemData root = new TreeItemData { Nombre = "Raíz", Ruta = "", EsDirectorio = true };
            root.TreeItems = new HashSet<TreeItemData>();

            foreach (OperationFolders itemData in elementos)
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
