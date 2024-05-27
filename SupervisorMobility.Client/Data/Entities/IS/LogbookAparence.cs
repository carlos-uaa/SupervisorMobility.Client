using SupervisorMobility.Client.Data.Entities.IS;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.Client.Data.Entities.IS

{
    public class LogbookAparence
    {
        public int LogbookAparenceId { get; set; }
        public int? Status { get; set; }
        public bool? IsActive { get; set; }

        public int? ApearanceId { get; set; }
        public Apearance? ApearanceInspection {  get; set; }

        public ICollection<DataPanelAnswer>? PanelResults { get; set; }
  = new List<DataPanelAnswer>();
              public ICollection<ProblemDefectAnswer>? ProblemDefectResults { get; set; }
  = new List<ProblemDefectAnswer>();


        public string? Programmed { get; set; }

        public int? InspectorId { get; set; }
        public User? Inspector { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? Date { get; set; }

        [Column(TypeName = "Time")]
        public TimeSpan? Time { get; set; }
    }
}
