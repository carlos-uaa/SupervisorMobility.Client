using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities.Hri
{
    public class HRIRevisionItems
    {
        [Key]
        public int ItemId { get; set; }
        public int HriId { get; set; }
        public HRI HRI { get; set; }
        public int ItemNumber { get; set; }
        public string  RevisionPoint { get; set; }
        public string RevisionMethod { get; set; } //por confirmar
        public string Veredict { get; set; }
        public string  Frecuency { get; set; }//por confirmar
    }
}
