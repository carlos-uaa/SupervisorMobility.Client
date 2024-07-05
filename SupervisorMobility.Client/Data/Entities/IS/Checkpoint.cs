
namespace SupervisorMobility.Client.Data.Entities.IS
{
    public class Checkpoint
    {
        public int CheckpointId { get; set; }

        public bool? IsActive { get; set; }

        //Formato tiene datos con tendencia a ser establecidos
        public int ItemOrder { get; set; }
        public string CheckpointTitle { get; set; } = string.Empty;
        public string CheckpointDescription { get; set; } = string.Empty;

        public ICollection<FileUpload>? Sketches { get; set; } = new List<FileUpload>();
        public List<CheckpointNorm>? Standars { get; set; } = new List<CheckpointNorm>();
    
    }
}
