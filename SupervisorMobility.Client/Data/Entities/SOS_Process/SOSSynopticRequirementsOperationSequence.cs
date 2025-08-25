namespace SupervisorMobility.Client.Data.Entities.SOS_Process
{
    public enum TypeOperacion
    {
        Person = -1,
        None = 0,
        Machine = 1,
    }
    public class SOSSynopticRequirementsOperationSequence
    {
        public int SOSSynopticRequirementsOperationSequenceId { get; set; }
        public int? Sequence { get; set; }


        public int? SOSHubId { get; set; }
        public SOSHub? SOSHub{ get; set; }


        public TypeOperacion? Type { get; set; }

        public string? TypeText { get; set; } = "";
        public bool? IsActive { get; set; }
    }
}