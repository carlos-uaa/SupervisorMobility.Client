using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities.Dtos.HRIDtos.HRIImagesDtos
{
    public class UpdateHRImageDto
    {
        public int HriId { get; set; }
        public int? ImageId { get; set; }
        public string ImageUrl { get; set; }
        public string ImageType { get; set; }
        public bool delete { get; set; } = false;
    }
}
