using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities.TreeStruct;
using System;
using System.Runtime.CompilerServices;
using static System.Net.WebRequestMethods;

namespace SupervisorMobility.Client.Services.TreeServices
{
    public class TreeService : ITreeService
    {
        //Service structure
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;
        private IBridgeCDMSService _CDMS;
        private IAssyChartService _assychartService;
        private ISnackbar _SnackbarService;
        private List<SOSCodePath>? _CodePaths { get; set; } = null;

        //Estructuras en crudo, sin procesar
        private CDMS_GOS_Directory raw_GOS { get; set; } = new CDMS_GOS_Directory();
        private CDMS_CCP_Directory raw_CCP { get; set; } = new CDMS_CCP_Directory();
        private CDMS_HOE_Directory raw_HOE { get; set; } = new CDMS_HOE_Directory();

        //Error Display Rutes Select ONLY
        private bool CCP_DataFound = true;
        private bool GOS_DataFound = true;
        private bool HOE_DataFound = true;

        //TreeItems Paths
        //root - estructura de todas las carpetas
        //node - nodo o carpeta especifica
        //CCP
        private TreeItemData Root_CCP { get; set; } = new TreeItemData();
        private TreeItemData Node_CCP { get; set; }
        private TreeItemData Node_CCP_CD { get; set; }
        //GOS
        private TreeItemData Root_GOS { get; set; } = new TreeItemData();
        private TreeItemData Node_GOS { get; set; }
        private TreeItemData Node_GOS_CD { get; set; }
        //HOE
        private TreeItemData Root_HOE { get; set; } = new TreeItemData();
        private TreeItemData Node_HOE { get; set; }
        private TreeItemData Node_HOE_CD { get; set; }



        //Files Path
        private CDMS_CCP_Archives? Files_In_Node_CCP;
        private CDMS_HOE_Archives? Files_In_Node_HOE;
        private CDMS_GOS_Archives? Files_In_Node_GOS;  
        //Comon Direction
        private CDMS_CCP_Archives? Files_In_Node_CCP_CD;
        private CDMS_HOE_Archives? Files_In_Node_HOE_CD;
        private CDMS_GOS_Archives? Files_In_Node_GOS_CD;
       


        public TreeService(HttpClient customHttpClientService, IJSRuntime jSRuntime, IBridgeCDMSService bridgeCDMS, IAssyChartService assyChartService, ISnackbar snackbar)
        {
            _http = customHttpClientService;
            _SnackbarService = snackbar;
            _CDMS = bridgeCDMS;
            _assychartService = assyChartService;
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<AsyncVoidMethodBuilder> InitializeTreeData()
        {
            //get raw data from CCP
            try
            {
                raw_CCP = await _CDMS.GetFoldersCCP();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Get raw data CCP From CDMS bridge");
                Console.WriteLine(ex.Message);
            }
            //transform raw data to treeitem & initialized bool
            if (raw_CCP != null)
            {
                CCP_DataFound = true;
                Root_CCP = Make_Tree_CCP(raw_CCP.operation);
            }
            else
            {
                CCP_DataFound = false;
            }
            //get raw data gos
            try
            {
                raw_GOS = await _CDMS.GetFoldersGOS();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Get raw data GOS From CDMS bridge");
                Console.WriteLine(ex.Message);
            }

            if (raw_GOS != null)
            {
                GOS_DataFound = true;
                Root_GOS = Make_Tree_GOS(raw_GOS.operation);
            }
            else
            {
                GOS_DataFound = false;
            }
            //gat raw data hoe
            try
            {
                raw_HOE = await _CDMS.GetFoldersHOE();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Get HOE Folder From CDMS");
                Console.WriteLine(ex.Message);
            }

            if (raw_HOE != null)
            {
                HOE_DataFound = true;
                Root_HOE = Make_Tree_HOE(raw_HOE.operation);
            }
            else
            {
                HOE_DataFound = false;
            }


            _CodePaths = await _assychartService.GetAllCodePaths();

            Console.WriteLine("Finish initialize TreeData");

            //_SnackbarService.Add("CDMS successfully ping.", Severity.Info);

            return new AsyncVoidMethodBuilder();
        }

        public TreeItemData Make_Tree_CCP(List<FolderCCP> elementos)
        {
            TreeItemData root = new TreeItemData { Name = "Raíz", Path = "", Is_Directory = true };
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
                    TreeItemData nodoActual = parent.TreeItems.ToList().Find(child => child.Name == nombre);

                    if (nodoActual == null)
                    {
                        // Si el nodo no existe, crearlo y agregarlo como hijo del padre actual
                        nodoActual = new TreeItemData() { Name = nombre, Path = itemData.ruta, Is_Directory = true };
                        nodoActual.TreeItems = new HashSet<TreeItemData>();
                        parent.TreeItems.Add(nodoActual);
                    }

                    // Actualizar el padre actual
                    parent = nodoActual;
                }

                // Agregar el nodo final (hoja)
                //TreeItemData hoja = new TreeItemData { Name = itemData.Name, Path = itemData.Path, Is_Directory= true };
                //hoja.TreeItems = null;
                //parent.TreeItems.Add(hoja);
            }

            // Imprimir el árbol
            return root;
        }
        public TreeItemData Make_Tree_HOE(List<FolderHOE> elementos)
        {
            TreeItemData root = new TreeItemData { Name = "Raíz", Path = "", Is_Directory = true };
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
                    TreeItemData nodoActual = parent.TreeItems.ToList().Find(child => child.Name == nombre);

                    if (nodoActual == null)
                    {
                        // Si el nodo no existe, crearlo y agregarlo como hijo del padre actual
                        nodoActual = new TreeItemData() { Name = nombre, Path = itemData.ruta, Is_Directory = true };
                        nodoActual.TreeItems = new HashSet<TreeItemData>();
                        parent.TreeItems.Add(nodoActual);
                    }

                    // Actualizar el padre actual
                    parent = nodoActual;
                }

                // Agregar el nodo final (hoja)
                //TreeItemData hoja = new TreeItemData { Name = itemData.Name, Path = itemData.Path, Is_Directory= true };
                //hoja.TreeItems = null;
                //parent.TreeItems.Add(hoja);
            }

            // Imprimir el árbol
            return root;
        }
        public TreeItemData Make_Tree_GOS(List<FolderGOS> elementos)
        {
            TreeItemData root = new TreeItemData { Name = "Raíz", Path = "", Is_Directory = true };
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
                    TreeItemData nodoActual = parent.TreeItems.ToList().Find(child => child.Name == nombre);

                    if (nodoActual == null)
                    {
                        // Si el nodo no existe, crearlo y agregarlo como hijo del padre actual
                        nodoActual = new TreeItemData() { Name = nombre, Path = itemData.ruta, Is_Directory = true };
                        nodoActual.TreeItems = new HashSet<TreeItemData>();
                        parent.TreeItems.Add(nodoActual);
                    }

                    // Actualizar el padre actual
                    parent = nodoActual;
                }

                // Agregar el nodo final (hoja)
                //TreeItemData hoja = new TreeItemData { Name = itemData.Name, Path = itemData.Path, Is_Directory= true };
                //hoja.TreeItems = null;
                //parent.TreeItems.Add(hoja);
            }

            // Imprimir el árbol
            return root;
        }

        public TreeItemData FindNodeByPath(TreeItemData rootNode, string path)
        {
            // Divide la ruta en partes
            string[] pathParts = path.Split('/');

            // Comienza desde la raíz del árbol
            TreeItemData currentNode = rootNode;

            foreach (string part in pathParts)
            {
                // Busca todos los nodos hijos con el nombre actual
                var matchingNodes = currentNode.TreeItems.Where(child => child.Name == part);

                // Si no se encuentra ningún nodo hijo con ese nombre, devuelve null
                if (matchingNodes.Count() == 0)
                {
                    return null;
                }

                // Si hay más de un nodo hijo con el mismo nombre, selecciona el mejor candidato
                if (matchingNodes.Count() > 1)
                {
                    int bestMatchLength = 0;
                    TreeItemData bestMatchNode = null;

                    foreach (var node in matchingNodes)
                    {
                        // Calcula la longitud común entre el path y el TreeItem.Path
                        int matchLength = LongestCommonPrefixLength(path, node.Path);

                        // Si es mejor que el mejor candidato actual, actualízalo
                        if (matchLength > bestMatchLength)
                        {
                            bestMatchLength = matchLength;
                            bestMatchNode = node;
                        }
                    }

                    // Usa el mejor candidato como el nodo actual
                    currentNode = bestMatchNode;
                }

                // Actualiza el nodo actual al nodo hijo seleccionado
                currentNode = matchingNodes.First();
            }

            // Devuelve el nodo encontrado
            return currentNode;
        }

        private int LongestCommonPrefixLength(string str1, string str2)
        {
            int minLength = Math.Min(str1.Length, str2.Length);
            int commonLength = 0;

            for (int i = 0; i < minLength; i++)
            {
                if (str1[i] == str2[i])
                {
                    commonLength++;
                }
                else
                {
                    break;
                }
            }

            return commonLength;
        }
        public async Task<AsyncVoidMethodBuilder> setNodesByPath(SOSCodePath codePath)
        {
            await Task.Run(() =>
            {
                Node_CCP = FindNodeByPath(Root_CCP, codePath.CCP);
                Node_GOS = FindNodeByPath(Root_GOS, codePath.GOS);
                Node_HOE = FindNodeByPath(Root_HOE, codePath.HOE);
                Node_CCP_CD = FindNodeByPath(Root_CCP, codePath.CommonDirectionCCP);
                Node_GOS_CD = FindNodeByPath(Root_GOS, codePath.CommonDirectionGOS);
                Node_HOE_CD = FindNodeByPath(Root_HOE, codePath.CommonDirectionHOE);
            }
            );
            return new AsyncVoidMethodBuilder();

        }

        public async Task<AsyncVoidMethodBuilder> GetFilesInNodeCCP(TreeItemData node)
        {
            node.showLoadingFiles = true;
            
            try
            {
                if (node.Path != "")
                {
                    if (node.Files_In_Node_CCP is null)
                    {
                        Console.WriteLine($"CCP {node.Path}");

                        node.Files_In_Node_CCP = new CDMS_CCP_Archives();
                        node.Files_In_Node_CCP = await _CDMS.GetFilesCCP(node.Path);

                        if (node.Files_In_Node_CCP == null)
                            node.Has_Files = false;
                        else
                            node.Has_Files = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error CCPFolderByDirectory: {ex.Message}");
            }
            finally
            {
                await Task.Run(() =>
                {
                  node.showLoadingFiles = false;
                }
            );
            }
            return new AsyncVoidMethodBuilder();
        }

        public async Task<AsyncVoidMethodBuilder> GetFilesInNodeGOS(TreeItemData node)
        {
            node.showLoadingFiles = true;

            try
            {
                if (node.Path != "")
                {
                    if (node.Files_In_Node_GOS is null)
                    {
                        Console.WriteLine($"GOS '{node.Path}'");

                        node.Files_In_Node_GOS = new CDMS_GOS_Archives();
                        node.Files_In_Node_GOS = await _CDMS.GetFilesGOS(node.Path);

                        if (node.Files_In_Node_GOS == null)
                            node.Has_Files = false;
                        else
                            node.Has_Files = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error GOS FolderByDirectory: {ex.Message}");
            }
            finally
            {
                await Task.Run(() =>
                {
                    node.showLoadingFiles = false;
                }
            );
            }
            return new AsyncVoidMethodBuilder();
        }

        public async Task<AsyncVoidMethodBuilder> GetFilesInNodeHOE(TreeItemData node)
        {
            node.showLoadingFiles = true;

            try
            {
                if (node.Path != "")
                {
                    if (node.Files_In_Node_HOE is null)
                    {
                        Console.WriteLine($"HOE '{node.Path}'");

                        node.Files_In_Node_HOE = new CDMS_HOE_Archives();
                        node.Files_In_Node_HOE = await _CDMS.GetFilesHOE(node.Path);

                        if (node.Files_In_Node_HOE == null)
                            node.Has_Files = false;
                        else
                            node.Has_Files = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error HOE FolderByDirectory: {ex.Message}");
            }
            finally
            {
                await Task.Run(() =>
                {
                    node.showLoadingFiles = false;
                }
            );
            }
            return new AsyncVoidMethodBuilder();
        }
        public bool HasCCP_Data()
        {
            return CCP_DataFound;
        }

        public bool HasGOS_Data()
        {
            return GOS_DataFound;
        }
        public bool HasHOE_Data()
        {
            return HOE_DataFound;
        }

        public async Task<SOSCodePath> getCodePath(int id)
        {
            return await Task.Run(() => _CodePaths.FirstOrDefault(c => c.SOSCodePathId == id));
        }
        public TreeItemData getRootCCP()
        {
            return this.Root_CCP;
        }
        public TreeItemData getRootGOS()
        {
            return this.Root_GOS;
        }
        public TreeItemData getRootHOE()
        {
            return this.Root_HOE;
        }
        public TreeItemData getNodeCCP()
        {
            return this.Node_CCP;
        }
        public TreeItemData getNodeGOS()
        {
            return this.Node_GOS;
        }
        public TreeItemData getNodeHOE()
        {
            return this.Node_HOE;
        } 
        public TreeItemData getNodeCCP_CD()
        {
            return this.Node_CCP_CD;
        }
        public TreeItemData getNodeGOS_CD()
        {
            return this.Node_GOS_CD;
        }
        public TreeItemData getNodeHOE_CD()
        {
            return this.Node_HOE_CD;
        }



    }
}
