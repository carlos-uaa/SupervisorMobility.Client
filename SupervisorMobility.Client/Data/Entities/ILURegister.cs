using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.Client.Data.Entities
{
    public class ILURegister
    {
        public int ILURegisterid { get; set; }

        public DateTime? AcquisitionDate { get; set; } = DateTime.Now;

        //public int? OperationId { get; set; }
        //public Operation? Operation { get; set; }
        public int? DistributionId { get; set; }
        public Distribution? Distribution { get; set; }

        public int? OperatorId { get; set; }
        public User? Operator { get; set; }


        public int? ILULevelId { get; set; }
        public ILULevel? ILULevel { get; set; }

        public bool isActive { get; set; }
    }
}
