namespace SupervisorMobility.Client.Data.Entities.CDMS
{
    public class CDMS_GOS_Document
    {
        public bool success { get; set; }
        public List<GOSDocument> operation { get; set; } = new List<GOSDocument>();
        public string message { get; set; }
    }
}
