using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities
{
    public class User
    {
        public int UserId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please indicate a payroll number")]
        public int Nomina { get; set; }
        [Required]
        public string Nombre { get; set; } = string.Empty;
        public bool? IsActive { get; set; }

        public int? PlantId { get; set; }
        public Plant? Plant { get; set; }
        public int? AreaId { get; set; }
        public Area? Area { get; set; }
        public int? GroupId { get; set; }
        public Group? Group { get; set; }

    }
}
