using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities
{
    public enum OperatorRole
    {
        SV,
        Lider,
        CA, // C/A
        NI
    }

    public class PatUserRole
    {
        public int PATId { get; set; }
        public int UserId { get; set; }
        public OperatorRole? Role { get; set; }
        public bool isActive { get; set; }

    }
}
