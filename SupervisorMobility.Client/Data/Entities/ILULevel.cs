using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities
{
    public class ILULevel
    {
        public int ILULevelId { get; set; }

        public string ILULevelCode { get; set; } = string.Empty;
        public string ILULevelDescription { get; set; } = string.Empty;
        
        public bool isActive { get; set; }

    }
}
