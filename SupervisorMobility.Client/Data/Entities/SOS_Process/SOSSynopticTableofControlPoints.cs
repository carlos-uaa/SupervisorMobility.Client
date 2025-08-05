using DocumentFormat.OpenXml.Office2010.PowerPoint;

namespace SupervisorMobility.Client.Data.Entities.SOS_Process
{
    public class SOSSynopticTableofControlPoints
    {
        public int SOSSynopticTableofControlPointsId { get; set; }

        public string? InternalControlNumber { get; set; }
        public string? ProcessName { get; set; }

      
        public int? CreatorId { get; set; }
        public User? Creator { get; set; }
        public int? ReviewerId { get; set; }
        public User? Reviewer { get; set; }
        public int? ApproverId { get; set; }
        public User? Approver { get; set; }
  

        public DateTime? CreatedAt { get; set; }

    

        public ICollection<SOSSynopticPointsLogbook>? SynopticPointsLogbooks { get; set; } = new List<SOSSynopticPointsLogbook>();


        public bool? IsActive { get; set; }
        //Es el id de sos hub que lo creeo y del que se trar la informacion
        public int? SOSHubId { get; set; }

        public IEnumerable<SOSHub>? SOSHubs { get; set; } = new List<SOSHub>();
        //las analisis y las secuencias de las que se sacaran los puntos principal
        public IEnumerable<SOSAnalysis>? Analyses { get; set; } = new List<SOSAnalysis>();
        public IEnumerable<SOSSequence>? Sequences { get; set; } = new List<SOSSequence>();
        
        //las pasos principales que se usan
        //public ICollection<SOSDistributionOperationSequence>? SOSDistributionOperationSequence { get; set; }

    }
}
