using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SupervisorMobility.Client.Data.Entities.IS

{
    public class LogbookTemplate
    {
        public int LogbookTemplateId { get; set; }
        public int? Status { get; set; }
        public bool? IsActive { get; set; }

        public int? TemplateId { get; set; }
        public Template? TemplateInspection {  get; set; }


        public DateTime? Date { get; set; }

        public TimeSpan? Time { get; set; }
        public string? RAN { get; set; }

        public ICollection<CheckpointNormAnswer>? CheckpointsResults { get; set; }
= new List<CheckpointNormAnswer>();

        public int? InspectorId { get; set; }
        public User? Inspector { get; set; }
        public string? InspectorSignature { get; set; }
        public DateTime? InspectorSignatureDate { get; set; }
        public ICollection<Commentary>? InspectorObservations { get; set; }
        public int? SupervisorId { get; set; }
        public User? Supervisor { get; set; }
        public string? SupervisorSignature { get; set; }
        public DateTime? SupervisorSignatureDate { get; set; }

      
    }
}
