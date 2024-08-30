namespace SupervisorMobility.Client.Data.Entities.SOS_Process
{
    public class Turn
    {
        public int TurnId { get; set; }

        public string? TurnType { get; set; }

        public int? OperatorId { get; set; }
        public User? Operator { get; set; }

        public int? SupervisorId { get; set; }
        public User? Supervisor { get; set; }
    }
}