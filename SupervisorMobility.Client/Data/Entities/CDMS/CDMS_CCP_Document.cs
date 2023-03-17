using SupervisorMobility.Client.Data.Entities.CDMS.Folders;

namespace SupervisorMobility.Client.Data.Entities.CDMS
{
    public class CDMS_CCP_Document
    {
        public bool success { get; set; }
        public List<CCPDocument> operation { get; set; } = new List<CCPDocument>();
        public string message { get; set; }
    }
}
