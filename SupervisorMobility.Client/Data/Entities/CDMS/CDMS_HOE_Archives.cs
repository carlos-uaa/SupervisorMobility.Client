namespace SupervisorMobility.Client.Data.Entities.CDMS
{
    public class CDMS_HOE_Archives
    {
        public bool success { get; set; }
        public List<HOEDocument> operation { get; set; } = new List<HOEDocument>();
        public string message { get; set; }
    }
}
