namespace SupervisorMobility.Client.Data.Entities
{
    public class Notification
    {
        public int NotificationID { get; set; }

        public string? MadeBy { get; set; }

        public string NotificationType { get; set; }
        public string NotificationText { get; set; }

        public User? User { get; set; }

        public bool IsAccepted { get; set; } = false;
        public bool IsActive { get; set; } = false;
        public DateTime EntryDate { get; set; }
    }
}
