
namespace SupervisorMobility.Client.Data.Entities.SOSAnalysis_Process
{
    public class Section
    {
        public int SectionId { get; set; }
        public List<Analysis> Analyses { get; set; } = new List<Analysis>();
        public string Step { get; set; } = "";
        public string Time { get; set; } = "";
        public bool? IsActive { get; set; }
    }
}