namespace SupervisorMobility.Client.Data.Entities.SOSAnalysis_Process
{
    public class MaterialUsed
    {
        public int MaterialUsedId { get; set; }

        public int MaterialId { get; set; }
        public Material Material { get; set; }

        public double Quantity { get; set; }

        public bool? IsActive { get; set; }
    }
}
