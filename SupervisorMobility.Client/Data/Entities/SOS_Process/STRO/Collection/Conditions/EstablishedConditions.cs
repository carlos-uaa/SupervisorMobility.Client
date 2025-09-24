
namespace SupervisorMobility.Client.Data.Entities.SOS_Process.STRO.Collection.Conditions
{
    public class EstablishedConditions
    {
        public int Id { get; set; }
        public string Condition { get; set; } = string.Empty;
        public int SOSSynopticTableofOperatingRequirementsId { get; set; }
        public int SectionId { get; set; }
    }
}