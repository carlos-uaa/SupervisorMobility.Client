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
        //Product info
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please indicate a Product/Model")]
        public int ProductId { get; set; }
        public Product Product { get; set; }
        //PLANT INFO
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please indicate a Plant")]
        public int PlantId { get; set; }
        public Plant Plant { get; set; }
        //AREA INFO
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please indicate a Area")]
        public int AreaId { get; set; }
        public Area Area { get; set; }
        //Distribution Info
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please indicate a Distribution")]
        public int DistributionId { get; set; }
        public Distribution Distribution { get; set; }
        //Operation Info
        public int OperationId { get; set; }
        //Data Operation to create
        [Required]
        public string OperationCode { get; set;}
        [Required]
        public string OperationDescription { get; set;}
        [Required]
        public bool? OperationIsActive { get; set; } = false;
        public Operation Operation { get; set; }


    }
}
