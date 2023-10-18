namespace SupervisorMobility.Client.Data.Entities.TreeStruct
{
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
}
