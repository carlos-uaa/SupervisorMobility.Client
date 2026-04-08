using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities.Hri
{
    public class HRILines
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string LineName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
