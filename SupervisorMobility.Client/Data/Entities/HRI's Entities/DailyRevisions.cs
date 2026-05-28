using SupervisorMobility.Client.Data.Entities.Dtos;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities.Hri
{
    public class DailyRevisions
    {
        [Key]
        public int RevisionId { get; set; }
        public int? RevisionCycleId { get; set; }
        public RevisionCycles? RevisionCycle { get; set; }
        public int? CycleId { get; set; }
        public HRICycles? cycle { get; set; }
        public int? HourmeterRevisionId { get; set; }
        public HourmeterRevision? HourmeterRevision { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int? UserId { get; set; }
        public User? Responsible { get; set; }
        public string? UserType { get; set; }
        public string? Status { get; set; }
        public bool? IsActive { get; set; }
    }
}
