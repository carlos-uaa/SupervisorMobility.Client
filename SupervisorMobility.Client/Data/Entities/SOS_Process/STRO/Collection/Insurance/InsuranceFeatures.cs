
namespace SupervisorMobility.Client.Data.Entities.SOS_Process.STRO.Collection.Insurance
{
    public class InsuranceFeatures
    {
        public int Id { get; set; }
        public string Insurance { get; set; } = string.Empty;
        public int SOSSynopticTableofOperatingRequirementsId { get; set; }
        public int SectionId { get; set; }
    }
}