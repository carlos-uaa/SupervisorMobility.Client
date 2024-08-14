using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.Client.Data.Entities.SOS_Process
{
    public class SOSSequenceLogbook
    {
        public int SOSSequenceLogbookId { get; set; }
        public int? Status { get; set; }
        public int? NoRevision { get; set; }
        public bool? IsActive { get; set; }

        public int SOSSequenceId { get; set; }
        public SOSSequence? SOSSequence { get; set; }

        public string? RevisedItem { get; set; }

        public int? SeniorSupervisorId { get; set; }
        public User? SeniorSupervisor { get; set; }
        public FileUpload? SeniorSupervisorSignatureImage { get; set; } = new();

        public int? SupervisorId { get; set; }
        public User? Supervisor { get; set; }
        public FileUpload? SupervisorSignatureImage { get; set; } = new();

        public DateTime? Date { get; set; }
    }
}
