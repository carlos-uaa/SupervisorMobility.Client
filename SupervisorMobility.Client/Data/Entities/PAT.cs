using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities
{
    public class PAT
    {
        public int PATid { get; set; }

        public int SupervisorId { get; set; }

        public User? Supervisor
        {
            get { return _supervisor; }
            set
            {
                _supervisor = value;
                if (_supervisor != null)
                {
                    SSVresponsibleID = _supervisor.SuperiorId;
                    AreaId = (int)_supervisor.AreaId;
                }
                else
                {
                    SSVresponsibleID = null;
                }
            }
        }
        private User? _supervisor;

        public int? SSVresponsibleID { get; set; }
        public User? SSVresponsible { get; set; }

        public int PlantId { get; set; }
        public Plant? Plant { get; set; }   

        public int AreaId { get; set; }
        public Area? Area { get; set; }

        public int DistributionId { get; set; }
        public Distribution? Distribution { get; set; }


        public DateTime? AplicationDate { get; set; }
        public int? AplicationYear
        {
            get { return AplicationDate?.Year; }
            set { AplicationDate = value != null ? new DateTime(value.Value, 1, 1) : null; }
        }


        public DateTime? CreationDate { get; set; }

        public DateTime? EditionDate { get; set; }

        public DateTime? ApprovalDate { get; set; }
    }
}
