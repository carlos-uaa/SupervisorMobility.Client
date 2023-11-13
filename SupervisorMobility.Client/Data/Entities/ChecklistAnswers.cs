namespace SupervisorMobility.Client.Data.Entities
{
    public class ChecklistAnswer
    {
        public int AnswerId { get; set; }
        public int JobObservationId { get; set; }
        public int QuestionID { get; set; }
        public string Prompt { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
    }
}
