using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace SupervisorMobility.Client.Data.Entities.IS

{
    public class ProblemDefectAnswer
    {
        public int ProblemDefectAnswerId { get; set; }

        public bool? IsActive { get; set; }

        //Aun por definir cual es el contenido de la casilla
        public string Result { get; set; } = string.Empty;

        public int? LogbookId { get; set; }
        public LogbookAparence? Logbook { get; set; }

        public int? ProblemId { get; set; }
        public ProblemDefect? Problem { get; set; }
    }
}
