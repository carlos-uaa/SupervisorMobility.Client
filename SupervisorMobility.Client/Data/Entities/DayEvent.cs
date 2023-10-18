namespace SupervisorMobility.Client.Data.Entities
{
    public class DayEvent
    {
        public int DayEventId { get; set; }
        public string Note { get; set; }
        public DateTime EventDate { get; set; } = new DateTime();
        public DateTime FromDate { get; set; } = new DateTime();
        public DateTime ToDate { get; set; } = new DateTime();

        public string DateValue { get; set; }
        public string DayName { get; set; }
        public string Message { get; set; }


    }
}