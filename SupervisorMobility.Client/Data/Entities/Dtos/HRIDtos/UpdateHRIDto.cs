using SupervisorMobility.Client.Data.Entites.Dtos.HRICyclesDtos;
using SupervisorMobility.Client.Data.Entites.Dtos.HRIRevisionItemsDtos;
using SupervisorMobility.Client.Data.Entities.Dtos.HRIDtos.HRIImagesDtos;

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
        public List<UpdateHRIRevisionItemDto> RevisionItems { get; set; } = new List<UpdateHRIRevisionItemDto>();
        public List<UpdateHRICycleDto> HRICycles { get; set; } = new List<UpdateHRICycleDto>();
        public List<UpdateHRImageDto>? Images { get; set; }
    }
}
