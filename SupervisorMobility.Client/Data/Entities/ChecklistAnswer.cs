using Microsoft.AspNetCore.Components.Forms;
using System.Text.Json.Serialization;

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
        
        [JsonIgnore]
        public List<IBrowserFile>? capturedImagesFiles { get; set; } = new();
        [JsonIgnore]
        public List<MemoryStream> NewFilesStreams { get; set; } = new();
        [JsonIgnore]
        public List<string> MediaUris = new();
        [JsonIgnore]
        public List<string> capturedImages = new List<string>();

       
    }
}
