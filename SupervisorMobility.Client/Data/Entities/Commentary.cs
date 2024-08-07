
namespace SupervisorMobility.Client.Data.Entities
{
    public class Commentary
    {
        public int CommentaryId { get; set; }
       
        public string? Comment { get; set; }

        public bool? IsActive { get; set; } = true;
    }
}
