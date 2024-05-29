using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities.IS

{
    public class ProblemDefect
    {
        public int ProblemDefectId { get; set; }

        public bool? IsActive { get; set; }

        //Aun por definir si son definidos o abiertos
        public int ItemOrder { get; set; }
        public string DefectDescription { get; set; } = string.Empty;



    }
}
