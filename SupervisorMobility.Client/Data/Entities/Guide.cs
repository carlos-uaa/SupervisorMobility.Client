using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities
{
    public class Guide
    {
        public int GuideId { get; set; }

        [Required]
        public string Code { get; set; } 
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; } 
        public bool? IsActive { get; set; } = false;

        public int FileUploadId { get; set; }
        public FileUpload? FileUpload { get; set; }
    }
}
