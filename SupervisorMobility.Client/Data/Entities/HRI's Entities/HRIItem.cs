using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities.Hri
{
    public class HRIItem
    {
        [Key]
        public int Id { get; set; }
        public string ControlNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
