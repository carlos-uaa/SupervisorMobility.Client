namespace SupervisorMobility.Client.Data.Entities
{
    public class FilesCDMS
    {
        public bool success { get; set; }
        public List<OperationFiles> operation { get; set; } = new List<OperationFiles>();
        public string message { get; set; }
    }
}
