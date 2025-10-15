namespace SupervisorMobility.Client.Data.Entities.SOS_Process.STRO.Collection.OperationM.Dtos
{
    public class OperationMachineDynamicDTO
    {
        public int SectionId { get; set; }
        public string InputOperationMachine{ get; set; } = string.Empty;
        public List<OperationMachine> OperationMachine { get; set; } = new List<OperationMachine>();
    }
}
