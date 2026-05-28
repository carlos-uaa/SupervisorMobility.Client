using SupervisorMobility.Client.Data.Entites.Dtos.HRICyclesDtos;
using SupervisorMobility.Client.Data.Entites.Dtos.HRIRevisionItemsDtos;
using SupervisorMobility.Client.Data.Entities.Dtos;
using SupervisorMobility.Client.Data.Entities.Dtos.HRIDtos;
using SupervisorMobility.Client.Data.Entities.Dtos.HRIDtos.HRIHourmeterRevisionDto;
using SupervisorMobility.Client.Data.Entities.Hri;

namespace SupervisorMobility.Client.Data.Entites.Dtos.HRIDtos
{
    public class GetHRIDto
    {
        public int HriId { get; set; }
        public int? HRILinesId { get; set; }
        public HRILines? Line { get; set; }
        public int? HRIItemId { get; set; }
        public HRIItem? NameOfItem { get; set; }
        public string? ControlNumber { get; set; }
        public int? HRIDockId { get; set; }
        public HRIDock? Dock { get; set; }
        public string? Department { get; set; }
        public List<HRImages>? Images { get; set; }
        public List<GetHRIRevisionItemDto>? ItemsRevised { get; set; }
        public List<WeeklyRevisions>? WeeklyRevisions { get; set; }
        public List<GetHRICyclesDto>? HriCycles { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public GetHourmeterRevisionDto? HourmeterRevision { get; set; }
        public int? SupervisorUserId { get; set; }
        public GetUserForHRIDailyRevsionDto? Supervisor { get; set; }
        public int? SSVUserId { get; set; }
        public GetUserForHRIDailyRevsionDto? SSV { get; set; }

        public int? PlantId { get; set; }
        public GetPlantForHRIDto? Plant { get; set; }
        public int? AreaId { get; set; }
        public GetAreaForHRIDto? Area { get; set; }
    }
}
