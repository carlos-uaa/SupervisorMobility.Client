using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities.Hri
{
    public class HRI
    {
        [Key]
        public int HriId { get; set; }
        public HRILines Line { get; set; }
        public HRIItem NameOfItem { get; set; }
        public string ControlNumber { get; set; }
        public HRIDock Dock { get; set; }
        public string Department { get; set; }
        public List<HRImages> Images { get; set; }
        public List<HRIRevisionItems> ItemsRevised { get; set; }
        public List<WeeklyRevisions> WeeklyRevisions { get; set; }
        public List<HRICycles> Cycles { get; set; }
        public List<RevisionCycles> Revisions { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
