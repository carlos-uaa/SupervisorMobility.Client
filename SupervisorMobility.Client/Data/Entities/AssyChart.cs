using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities
{
    public class AssyChart
    {
        public int AssyChardId { get; set; }
        [Required]
        public bool? IsActive { get; set; } = false;
        [Required]
        public string GOS { get; set; }
        [Required]
        public string CCP { get; set; }
        [Required]
        public string HOE { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
        //Linkers
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int PlantId { get; set; }
        [Required]
        public int AreaId { get; set; }
        [Required]
        public int DistributionId { get; set; }
        public int OperationId { get; set; }
        [Required]

        //Data Operation to create
        public string CodeOperation { get; set; } = string.Empty;
        [Required]

        public string DescriptionOperation { get; set; } = string.Empty;
        [Required]

        public bool? IsActiveOperation { get; set; } = false;


    }
}
