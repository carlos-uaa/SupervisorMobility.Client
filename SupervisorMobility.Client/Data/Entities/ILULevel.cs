using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities
{
    public class ILULevel
    {
        public int ILULevelId { get; set; }

        public char ILULevelCode { get; set; }
        public string ILULevelDescription { get; set; } = string.Empty;
        
        public bool isActive { get; set; }

    }
}
