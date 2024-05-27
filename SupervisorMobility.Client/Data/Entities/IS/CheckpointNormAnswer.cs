using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace SupervisorMobility.Client.Data.Entities.IS

{
    public class CheckpointNormAnswer
    {
        public int CheckpointNormAnswerId { get; set; }

        public bool? IsActive { get; set; }

        //Aun por definir cual es el contenido de la casilla
        public string Result { get; set; } = string.Empty;
        public int? LogbookTemplateId { get; set; }
        public LogbookTemplate? LogbookTemplate { get; set; }

        public int? CheckpointNormId { get; set; }
        public CheckpointNorm? CheckpointNorm { get; set; }

    }
}
