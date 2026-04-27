using SupervisorMobility.Client.Data.Entites.Dtos.HRICyclesDtos;
using SupervisorMobility.Client.Data.Entites.Dtos.HRIRevisionItemsDtos;

namespace SupervisorMobility.Client.Data.Entites.Dtos.HRIDtos
{
    public class UpdateHRIDto
    {
        public int? HRILinesId { get; set; }       
        public int? HRIItemId { get; set; }
        public string? ControlNumber { get; set; }
        public int? HRIDockId { get; set; }
        public string? Department { get; set; }
        public int? SupervisorUserId { get; set; }
        public int? SSVUserId { get; set; }
        public List<UpdateHRIRevisionItemDto> ItemsRevised { get; set; } = new List<UpdateHRIRevisionItemDto>();
        public List<UpdateHRICycleDto> HriCycles { get; set; } = new List<UpdateHRICycleDto>();
    }
}
