using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities
{
    public class Glosary
    {

        public int GlosaryWordId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
    }
}
