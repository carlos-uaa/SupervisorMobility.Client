namespace SupervisorMobility.Client.Data.Entities.SOSAnalysis_Process
{
    public class SpecialCaseAbnormalSituation
    {
        public int SpecialCaseAbnormalSituationId { get; set; }
        public string key { get; set; }

        public string PartName { get; set; }
        public string PartNumber { get; set; }

        public int? PartId { get; set; }
        public Part? Part { get; set; }

        public double Quantity { get; set; }

        public bool? IsActive { get; set; }
    }
}
