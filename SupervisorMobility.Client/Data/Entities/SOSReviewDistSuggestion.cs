namespace SupervisorMobility.Client.Data.Entities
{
    public class SOSReviewDistSuggestion
    {
        public int SOSReviewDistSuggestionId { get; set; }

        public int? SOSReviewProgramid { get; set; }
        public SOSReviewProgram? SOSReviewProgram { get; set; }

        public int DistributionId { get; set; }
        public Distribution? Distribution { get; set; }

        public bool SuggestionApplied { get; set; }
    }
}
