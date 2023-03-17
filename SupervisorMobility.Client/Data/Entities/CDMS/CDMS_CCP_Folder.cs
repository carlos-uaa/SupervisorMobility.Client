using SupervisorMobility.Client.Data.Entities.CDMS.Folders;

namespace SupervisorMobility.Client.Data.Entities.CDMS
{
    public class CDMS_CCP_Folder
    {
        public bool success { get; set; }
        public List<FolderCCP> operation { get; set; } = new List<FolderCCP>();
        public string message { get; set; }
    }
}
