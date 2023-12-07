using Microsoft.AspNetCore.Components.Forms;

namespace SupervisorMobility.Client.Data.Entities
{
    public class ChecklistAnswer
    {
        public int AnswerId { get; set; }
        public int JobObservationId { get; set; }
        public int QuestionID { get; set; }
        public string Prompt { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;

        public string? CommentarySV { get; set; } = string.Empty;
        public string? CommentarySSV { get; set; } = string.Empty;
        public ICollection<FileUpload>? Evidences { get; set; } = new List<FileUpload>();


        public bool Show { get; set; } = false;
        public bool Edited { get; set; } = false;
        public List<IBrowserFile>? capturedImagesFiles { get; set; } = new();
        public List<MemoryStream> NewFilesStreams { get; set; } = new();
        public List<string> MediaUris = new();

        public List<string> capturedImages = new List<string>();

       
    }
}
