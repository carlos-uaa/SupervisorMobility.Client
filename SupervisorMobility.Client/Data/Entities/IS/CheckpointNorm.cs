using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace SupervisorMobility.Client.Data.Entities.IS
{
    public class CheckpointNorm
    {
        public int CheckpointNormId { get; set; }

        public bool? IsActive { get; set; }
        public bool NA { get; set; }

        //Formato tiene datos con tendencia a ser establecidos
        public int ItemOrder { get; set; }
        public string Standard { get; set; } = string.Empty;

        public ICollection<FileUpload>? Sketches { get; set; } = new List<FileUpload>();

    }
}
