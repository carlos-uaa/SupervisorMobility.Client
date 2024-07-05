using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities.IS
{
    public class DataPanel
    {
        public int DataPanelId { get; set; }

        public bool? IsActive { get; set; }

        //Formato tiene datos con tendencia a ser establecidos
        public int ItemOrder { get; set; }
        public string DataTitle { get; set; } = string.Empty;

        public ICollection<DataPanelSpecification>? Specifications { get; set; }
        = new List<DataPanelSpecification>();

        //variables para drop zone re-order sequence
        public string Container { get; set; } = "CategoryContainer";

    }
}
