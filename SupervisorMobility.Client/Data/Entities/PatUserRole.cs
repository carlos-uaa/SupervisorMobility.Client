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
        public int PatUserRoleId { get; set; }
        public int PATId { get; set; }
        public int UserId { get; set; }
        public OperatorRole? Role { get; set; }
        public string? Comment { get; set; }
        public bool IsActive { get; set; }

    }
}
