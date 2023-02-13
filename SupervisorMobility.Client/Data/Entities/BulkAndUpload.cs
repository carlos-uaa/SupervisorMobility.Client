using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities
{
    public class BulkAndUpload
    {
        public int? AssyChardId { get; set; }
       
        public bool? IsActive { get; set; } = false;
        public string? GOS { get; set; }
        public string? CCP { get; set; }
        public string? HOE { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        //Linkers
        //Product info
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
        //PLANT INFO
        public int? PlantId { get; set; }
        public Plant? Plant { get; set; }
        //AREA INFO
        public int? AreaId { get; set; }
        public Area? Area { get; set; }
        //Distribution Info
        public int? DistributionId { get; set; }
        public Distribution? Distribution { get; set; }
        //Operation Info
        public int? OperationId { get; set; }
        public Operation? Operation { get; set; }

    }

}
