namespace SupervisorMobility.Client.Data.Entities.CDMS
{
    public class CDMS_GOS_Folder
    {
        public bool success { get; set; }
        public List<FolderGOS> operation { get; set; } = new List<FolderGOS>();
        public string message { get; set; }
    }
}
