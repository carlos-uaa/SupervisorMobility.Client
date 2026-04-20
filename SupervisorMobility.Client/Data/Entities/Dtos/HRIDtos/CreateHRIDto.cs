using SupervisorMobility.Client.Data.Entites.Dtos.HRICyclesDtos;
using SupervisorMobility.Client.Data.Entities.Dtos.HRIDtos.HRIHourmeterRevisionDto;
using SupervisorMobility.Client.Data.Entites.Dtos.HRIRevisionItemsDtos;
using SupervisorMobility.Client.Data.Entities.Dtos.HRIDtos.HRIImagesDtos;
using SupervisorMobility.Client.Data.Entities.Hri;

namespace SupervisorMobility.Client.Data.Entites.Dtos.HRIDtos
{
    public class CreateHRIDto
    {
        public int? HRILinesId { get; set; }
        public HRILines? Line { get; set; }
        public int? HRIItemId { get; set; }
        public HRIItem? NameOfItem { get; set; }
        public string? ControlNumber { get; set; }
        public int? HRIDockId { get; set; }
        public HRIDock? Dock { get; set; }
        public string? Department { get; set; }
        public List<CreateHRImageDto>? Images { get; set; }
        public List<CreateHRIRevisionItemDto>? ItemsRevised { get; set; }
        public List<WeeklyRevisions>? WeeklyRevisions { get; set; }
        public List<CreateHRICyclesDto>? HriCycles { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public CreateHourMeterRevisionDto? HourmeterRevision { get; set; }
        public int? SupervisorId { get; set; }
    }
}
