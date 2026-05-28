using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities.Dtos.HRIDtos.HRIImagesDtos
{
    public class CreateHRImageDto
    {
        public int HriId { get; set; }
        public string ImageUrl { get; set; }
        public string ImageType { get; set; }
    }
}
