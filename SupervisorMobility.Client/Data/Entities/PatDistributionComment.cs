using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities
{

    public class PatDistributionComment
    {
        public int PatDistributionCommentId { get; set; }
        public int PATId { get; set; }
        public int DistributionId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public bool IsActive { get; set; }

    }
}
