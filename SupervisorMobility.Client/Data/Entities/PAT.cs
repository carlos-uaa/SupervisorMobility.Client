using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities
{
    public class PAT
    {
        public int PATid { get; set; }
        public int Status { get; set; }
        public int SupervisorId { get; set; }
        public User? Supervisor { get; set; }


        public int? SSVresponsibleID { get; set; }
        public User? SSVresponsible { get; set; }

        public int PlantId { get; set; }
        public Plant? Plant { get; set; }   

        public int AreaId { get; set; }
        public Area? Area { get; set; }

        //public int DistributionId { get; set; }
        //public Distribution? Distribution { get; set; }

        public ICollection<LeadershipRecord>? LeadershipRecords { get; set; }
        public ICollection<PatUserRole>? PatUserRoles { get; set; }
        public ICollection<PatDistributionComment>? PatDistributionComments { get; set; }

        public int? KnowledgePercentage { get; set; }

        public DateTime? AplicationDate { get; set; }
        public int? AplicationYear { get; set; }
 

        public DateTime? CreationDate { get; set; }

        public DateTime? EditionDate { get; set; }

        public DateTime? ApprovalDate { get; set; }
        public int? SOSHubId { get; set; }
        public SOSHub? SOSHub { get; set; }
        public bool IsActive { get; set; }
    }
}
