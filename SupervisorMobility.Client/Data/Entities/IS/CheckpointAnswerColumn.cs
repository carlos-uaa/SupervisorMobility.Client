namespace SupervisorMobility.Client.Data.Entities.IS
{
    public class CheckpointAnswerColumn
    {
        public int ColumnId { get; set; }

        public DateTime? Date { get; set; }

        public string? RAN { get; set; }

        public List<CheckpointNormAnswer>? CheckpointsResults { get; set; }
        = new List<CheckpointNormAnswer>();

        public int? InspectorId { get; set; }
        public User? Inspector { get; set; }
        public Commentary? InspectorObservations { get; set; }
        public FileUpload? InspectorsSignature { get; set; } = new();

    }
}
