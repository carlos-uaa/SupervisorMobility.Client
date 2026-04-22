using SupervisorMobility.Client.Data.Entites.Dtos.HRICyclesDtos;
using SupervisorMobility.Client.Data.Entites.Dtos.HRIRevisionItemsDtos;
using SupervisorMobility.Client.Data.Entities.Dtos;
using SupervisorMobility.Client.Data.Entities.Dtos.HRIDtos;
using SupervisorMobility.Client.Data.Entities.Dtos.HRIDtos.HRIHourmeterRevisionDto;
using SupervisorMobility.Client.Data.Entities.Hri;

namespace SupervisorMobility.Client.Data.Entites.Dtos.HRIDtos
{
    public class HRIToTableDto
    {
        public int HriId { get; set; }
        public HRILines? Line { get; set; }
        public HRIItem? NameOfItem { get; set; }
        public string? ControlNumber { get; set; }
        public string? Department { get; set; }
        public int ImagesCount { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
