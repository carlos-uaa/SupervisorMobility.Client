using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities
{
    public class FileUpload
    {
        public int FileUploadId { get; set; }
        public string? FileName { get; set; }
        public string? StorageFileName { get; set; }
        public string? ContentType { get; set; }
        public DateTime UploadDate { get; set; }
        public int? LupId { get; set; }
    }
}
