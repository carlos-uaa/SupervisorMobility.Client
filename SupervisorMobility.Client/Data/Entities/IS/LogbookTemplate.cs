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

        public List<CheckpointAnswerColumn>? CheckpointsResults { get; set; }
        = new List<CheckpointAnswerColumn>();

        public int? SupervisorId { get; set; }
        public User? Supervisor { get; set; }
        public FileUpload? SupervisorSignatureImage { get; set; } = new();


    }
}
