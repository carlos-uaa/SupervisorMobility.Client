namespace SupervisorMobility.Client.Data.Entities.Dtos
{
    public class AnalysisSequencesDto
    {
        public List<SOSSequence> SequencesSelected { get; set; } = new();
        public List<SOSAnalysis> AnalysisSelected { get; set; } = new();
    }
}
