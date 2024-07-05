namespace SupervisorMobility.Client.Data.Entities.IS

{
    public class Part
    {
        public int PartId { get; set; }
        public bool? IsActive { get; set; }

        public string? PartName { get; set; } = string.Empty;
        public string? PartNumber { get; set; } = string.Empty;

        public int ModelId { get; set; }
        public Product? Model { get; set; }

        public ICollection<FileUpload> Sketches { get; set; } = new List<FileUpload>();

    }
}
