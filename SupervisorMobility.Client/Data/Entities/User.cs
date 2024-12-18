using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SupervisorMobility.Client.Data.Entities
{
    public class User
    {
        public int UserId { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Please indicate a payroll number")]
        public int? Payroll { get; set; }
        public string? ObjectId { get; set; }
        [Required(ErrorMessage = "Please indicate a Name")]
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public int UserType { get; set; }

        public string? Management { get; set; }
        public string? Department { get; set; }
        public string? Process { get; set; }

        public int? SuperiorId { get; set; }
        public User? Superior { get; set; }

        public ICollection<User>? Subordinates { get; set; }
        public ICollection<ILURegister>? ILURegisers { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdated { get; set; }
        public DateTime? DisabledDate { get; set; }
        public bool? IsActive { get; set; } = false;

        public DateTime? IncomesDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? ProfilePictureId { get; set; }
        public FileUpload? ProfilePicture { get; set; }


        public int? PlantId { get; set; }
        public Plant? Plant { get; set; }
        
  
        public int? AreaId { get; set; }
        public Area? Area { get; set; }

        public ICollection<Area>? Areas { get; set; }


        public int? GroupId { get; set; }
        public Group? Group { get; set; }

        public int? HciId { get; set; }
        public HCI? Hci { get; set; }

        public int? DistributionId { get; set; }
        public Distribution? Distribution { get; set; }

        public int Sequence { get; set; } = 1; 
        public string Container { get; set; } = "CategoryContainer";

        public ICollection<SOSHub>? SosHubs = new List<SOSHub>();
    }
}
