namespace SupervisorMobility.Client.Data.Entities
{
    public class AssyChart
    {
        public int AssyChardId { get; set; }
        public bool? IsActive { get; set; } = false;
        public string GOS { get; set; } = string.Empty;
        public string CCP { get; set; } = string.Empty;
        public string HOE { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        //Linkers
        public int PlantId { get; set; }
        public int AreaId { get; set; }
        public int DistributionId { get; set; }
        public int OperacionId { get; set; }
        public Operation? Operation { get; set; } = new();


    }
}
