namespace SupervisorMobility.Client.Data.Entities
{
    public class FoldersCDMS
    {
        public bool success { get; set; }
        public List<OperationFolders> operation { get; set; } = new List<OperationFolders>();
        public string message { get; set; }
    }

}

