using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities
{
    public class User
    {
        public int UserId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please indicate a payroll number")]
        public int Payroll { get; set; }
        [Required]
        public string ObjectId { get; set; }

        public string Name { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public bool IsSupervisor { get; set; }
        public bool IsOperator { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdated { get; set; }
        public DateTime? DisabledDate { get; set; }
        public bool? IsActive { get; set; } = false;

        public int PlantId { get; set; }
        public Plant? Plant { get; set; }
        public int AreaId { get; set; }
        public Area? Area { get; set; }
        public int GroupId { get; set; }
        public Group? Group { get; set; }

      

    }
}
