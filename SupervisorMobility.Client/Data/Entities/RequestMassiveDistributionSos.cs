using static SupervisorMobility.Client.Pages.Inicio.SOSProgramPage.SOS_Details;

namespace SupervisorMobility.Client.Data.Entities
{
    public class RequestMassiveDistributionSos
    {
        public List<DistSelect> distributions { get; set; } = new List<DistSelect>();
        public List<JobObservationNulls> Jobs { get; set; } = new List<JobObservationNulls> { };
    }
}
