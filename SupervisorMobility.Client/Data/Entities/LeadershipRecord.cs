using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.Client.Data.Entities
{
    public class LeadershipRecord
    {
        public int LeadershipRecordsid { get; set; }

        public DateTime? AcquisitionDate { get; set; } = DateTime.Now;

        public int? DistributionId { get; set; }
        public Distribution? Distribution { get; set; }

        public int? OperatorId { get; set; }
        public User? Operator { get; set; }

        public int? ILULevelId { get; set; }
        public ILULevel? ILULevel { get; set; }

        public bool isActive { get; set; }
    }
}
