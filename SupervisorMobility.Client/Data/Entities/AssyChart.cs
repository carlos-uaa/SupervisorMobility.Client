namespace SupervisorMobility.Client.Data.Entities
{
    public class AssyChart
    {
        public int AssyChardId { get; set; }
        public bool? IsActive { get; set; } = false;
        public string GOS { get; set; } = string.Empty;
        public string CCP { get; set; } = string.Empty;
        public string HOE { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
        //Linkers
        public int ProductId { get; set; }
        public int PlantId { get; set; }
        public int AreaId { get; set; }
        public int DistributionId { get; set; }
        //Data Operation to create
        public string CodeOperation { get; set; } = string.Empty;
        public string DescriptionOperation { get; set; } = string.Empty;
        public bool? IsActiveOperation { get; set; } = false;


    }
}
