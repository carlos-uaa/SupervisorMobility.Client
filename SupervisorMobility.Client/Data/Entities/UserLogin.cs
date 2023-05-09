using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SupervisorMobility.Client.Data.Entities
{
    public class UserLogin
    {

        [Required(ErrorMessage = "Please indicate a Name")]
        public string Username { get; set; } = string.Empty;
        [Required(ErrorMessage = "Please indicate a Password")]
        public string Password { get; set; } = string.Empty;

    }
}
