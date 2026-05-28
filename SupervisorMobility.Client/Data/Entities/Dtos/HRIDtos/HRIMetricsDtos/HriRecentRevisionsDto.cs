namespace SupervisorMobility.Client.Data.Entities.Dtos.HRIDtos.HRIMetricsDtos
{
    public class HriRecentRevisionsDto
    {
        public int RevisionId { get; set; }
        public int HriId { get; set; }
        public string HRIName { get; set; }
        public string RevisionPointName { get; set; }
        public string Line { get; set; }
        public int Cycle { get; set; }
        public int Day { get; set; }
        public string Month { get; set; }
        public string Status { get; set; }
    }
}
