namespace SupervisorMobility.Client.Data.Entities.TreeStruct
{
    public class TreeItemData
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool Is_Directory { get; set; }
        public bool IsExpanded { get; set; }
        public HashSet<TreeItemData> TreeItems { get; set; } = new HashSet<TreeItemData>();
        public CDMS_CCP_Archives? Files_In_Node_CCP;
        public CDMS_HOE_Archives? Files_In_Node_HOE;
        public CDMS_GOS_Archives? Files_In_Node_GOS;

        public bool Has_Files = false;
        public bool showLoadingFiles = true;
        public TreeItemData()
        {
            TreeItems = new HashSet<TreeItemData>();
        }


        public TreeItemData(string nombre, string ruta, bool esDirectorio)
        {
            Name = nombre;
            Path = ruta;
            Is_Directory = esDirectorio;
            TreeItems = new HashSet<TreeItemData>();
        }


        public void AgregarNodo(TreeItemData nodo)
        {
            TreeItems.Add(nodo);
        }

    }
}
