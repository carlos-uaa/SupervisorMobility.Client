

namespace SupervisorMobility.Client.Data.Entities
{
    public class HCICategory
    {

        public int HCICategoryId { get; set; }
        public int ChosenCategoryDepartmentId { get; set; }
        public Department ChosenCategory { get; set; }
        public DateTime? Date { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}