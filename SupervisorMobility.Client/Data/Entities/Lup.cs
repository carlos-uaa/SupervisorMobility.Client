using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities
{
    public class Lup
    {

        public int LupId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please indicate a Job Observation")]
        public int JobObservationId { get; set; }
        public string Oportunity { get; set; }

        public bool IsActive { get; set; }
        public string? Observer { get; set; }
        public int Pillar { get; set; }
        public string? Q3 { get; set; }
        public string? Q4 { get; set; }
        public int? Status { get; set; }

        //Evidence
        //public ICollection<FileUpload> Evidences { get; set; }
        //    = new List<FileUpload>();

        [Required]
        public DateTime? CreatedDate { get; set; }
        [Required]
        public DateTime? EndDate { get; set; }


    }
}
