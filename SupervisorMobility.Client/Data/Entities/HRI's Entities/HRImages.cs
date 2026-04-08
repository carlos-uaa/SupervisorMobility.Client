using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities.Hri
{
    public class HRImages
    {
        [Key]
        public int ImageId { get; set; }
        public int HriId { get; set; }
        public string ImageUrl { get; set; }
        public string ImageType { get; set; }
    }
}
