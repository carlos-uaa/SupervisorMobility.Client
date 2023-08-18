
namespace SupervisorMobility.Client.Data.Entities
{
    public class RouteProductAssyChart
    {
        public int RouteProductAssyChartId { get; set; }

        public string? GOS { get; set; } = string.Empty;
        public string? CCP { get; set; } = string.Empty;
        public string? HOE { get; set; } = string.Empty;

        public int AssyChardId { get; set; }
        public AssyChart? AssyChart { get; set; }

        public int? ProductId { get; set; }
        public Product? Product { get; set; }

        public bool? IsActive { get; set; }

    }
}
