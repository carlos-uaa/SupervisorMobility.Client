using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities
{
    public class HCIILU
    {
        public int ID { get; set; }

        public DateTime? Start {  get; set; }
        public DateTime? End { get; set; }
        public string Description { get; set; }
        public string level { get; set; }

        //public HCI? _HCI { get; set; }
        public int? RegisterILURegisterid { get; set; }
        public ILURegister? Register { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
