using SupervisorMobility.Client.Data.Entities.CDMS.Downloads;

namespace SupervisorMobility.Client.Data.Entities.CDMS
{
    public class CDMS_DownloadFile
    {
        public bool success { get; set; }
        public Download_CDMS_Document operation { get; set; } = new Download_CDMS_Document();
        public string message { get; set; }
    }
}
