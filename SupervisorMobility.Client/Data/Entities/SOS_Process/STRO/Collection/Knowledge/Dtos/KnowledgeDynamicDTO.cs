namespace SupervisorMobility.Client.Data.Entities.SOS_Process.STRO.Collection.Knowledge.Dtos
{
    public class KnowledgeDynamicDTO
    {
        public int SOSHubId { get; set; }
        public string SelectedKnowledgeName { get; set; } = string.Empty;

        public List<Knowledge> FilteredKnowledge { get; set; } = new();

        public List<int> KnowledgeIds { get; set; } = new();
    }
}
