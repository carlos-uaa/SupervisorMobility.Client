using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities.Hri
{
    public class RevisionMethod
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public List<HRIRevisionItems> HRIRevisionItems { get; set; }
    }
}
