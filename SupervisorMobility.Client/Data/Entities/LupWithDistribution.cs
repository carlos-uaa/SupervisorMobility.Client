using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities
{
    public class LupWithDistribution
    {

        public Lup Lup { get; set; }
        public string? Distribution = string.Empty;


    }
}
