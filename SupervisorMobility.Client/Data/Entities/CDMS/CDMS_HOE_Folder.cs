namespace SupervisorMobility.Client.Data.Entities.CDMS
{
    public class CDMS_HOE_Folder
    {
        public bool success { get; set; }
        public List<FolderHOE> operation { get; set; } = new List<FolderHOE>();
        public string message { get; set; }
    }
}
