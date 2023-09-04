namespace SupervisorMobility.Client.Data.Entities
{
    public class SOSCodePath
    {
        public int SOSCodePathId { get; set; }


        public string Code { get; set; } = string.Empty;
      
        public string? GOS { get; set; } = string.Empty;
        public string? CommonDirectionGOS { get; set; } = string.Empty;
        public string? CCP { get; set; } = string.Empty;
        public string? CommonDirectionCCP { get; set; } = string.Empty;
        public string? HOE { get; set; } = string.Empty;
        public string? CommonDirectionHOE { get; set; } = string.Empty;

        public int DistributionId { get; set; }
        public Distribution? Distribution { get; set; }

        public int? ProductId { get; set; }
        public Product? Product { get; set; }

        public bool? IsActive { get; set; }
    }
}
