using SupervisorMobility.Client.Data.Entites.Dtos.HRIRevisionCycles;
using SupervisorMobility.Client.Data.Entities.Hri;

namespace SupervisorMobility.Client.Data.Entites.Dtos.HRIRevisionItemsDtos
{
    public class GetHRIRevisionItemDto
    {
        public int ItemId { get; set; }
        public int HriId { get; set; }
        public int ItemNumber { get; set; }
        public string RevisionPoint { get; set; }
        public int? RevisionMethodId { get; set; }
        public GetRevisionMethodDto? RevisionMethod { get; set; }
        public int? VeredictId { get; set; }
        public GetVeredictDto? Veredict { get; set; }
        public int? FrequencyId { get; set; }
        public GetFrequencyDto? Frequency { get; set; }
        public List<GetRevisionCyclesDto>? RevisionCycles { get; set; }
        public bool? IsActive { get; set; }
    }
}
