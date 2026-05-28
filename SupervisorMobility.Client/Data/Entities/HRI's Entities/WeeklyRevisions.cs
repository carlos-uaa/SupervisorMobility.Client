using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities.Hri
{
    public class WeeklyRevisions
    {
        [Key]
        public int RevisionId { get; set; }
        public int HriId { get; set; }
        public HRI HRI { get; set; }
        public int? UserId { get; set; }
        public User? SeniorSupervisor { get; set; }
        public int Month { get; set; }
        public int Week { get; set; }
        public bool? IsActive { get; set; }
        public string? Status { get; set; }
    }
}
