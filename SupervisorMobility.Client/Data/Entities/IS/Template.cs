using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace SupervisorMobility.Client.Data.Entities.IS

{
    public class Template
    {
        public int TemplateId { get; set; }
        public bool? IsActive { get; set; }

        public int? PartId { get; set; }
        public Part? Part { get; set; }

        //Item de la categoria
        public List<Checkpoint>? CheckpointItems { get; set; }
          = new List<Checkpoint>();
        //specificacion de la categoria
        //public ICollection<CheckpointNorm>? CheckpointNormItems { get; set; }
        //  = new List<CheckpointNorm>();


        public DateTime? CreatedDate { get; set; }
        public DateTime? CheckDate { get; set; }
        public DateTime? FinishedDate { get; set; }


    }
}
