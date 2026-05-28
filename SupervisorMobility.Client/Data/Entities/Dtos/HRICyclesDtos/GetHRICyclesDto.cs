
using SupervisorMobility.Client.Data.Entities.Dtos.HRIDtos;
using SupervisorMobility.Client.Data.Entities.Hri;

namespace SupervisorMobility.Client.Data.Entites.Dtos.HRICyclesDtos
{
    public class GetHRICyclesDto
    {
        public int CycleId { get; set; }
        public int HriId { get; set; }
        public int Cycle { get; set; }
        public bool? IsActive { get; set; }
        public int? SupervisorUserId { get; set; }
        public GetUserForHRIDailyRevsionDto? Supervisor { get; set; }
        public int? OperatorUserId { get; set; }
        public GetUserForHRIDailyRevsionDto? Operator { get; set; } = null;
        public string? UserType { get; set; }
        public List<GetDailyRevisionDto>? DailyRevisions { get; set; }
    }
}
