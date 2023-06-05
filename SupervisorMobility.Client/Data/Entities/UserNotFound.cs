using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SupervisorMobility.Client.Data.Entities
{
    public class UserNotFound
    {
        public int UserNotFoundId { get; set; }
        public string? ObjectId { get; set; }
        public string? Name { get; set; }
        public bool? IsActive { get; set; } = false;
    }
}
