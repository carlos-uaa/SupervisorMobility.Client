
namespace SupervisorMobility.Client.Data.Entities.SOS_Process.STRO.Collection.Conditions
{
    public class OperationMachine
    {
        public int Id { get; set; }
        public string Operation { get; set; } = string.Empty;
        public int SOSSynopticTableofOperatingRequirementsId { get; set; }
        public int SectionId { get; set; }
    }
}