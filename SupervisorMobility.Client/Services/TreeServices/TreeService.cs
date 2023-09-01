using SupervisorMobility.Client.Data.Entities.TreeStruct;

namespace SupervisorMobility.Client.Services.TreeServices
{
    public class TreeService: ITreeService
    {


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
