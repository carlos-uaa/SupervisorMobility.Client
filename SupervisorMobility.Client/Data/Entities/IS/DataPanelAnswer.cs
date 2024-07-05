
namespace SupervisorMobility.Client.Data.Entities.IS

{
    public class DataPanelAnswer
    {
        public int DataPanelAnswerId { get; set; }

        public bool? IsActive { get; set; }

        //Aun por definir cual es el contenido de la casilla
        public string Result { get; set; } = string.Empty;
        public int? LogbookId { get; set; }
        public LogbookAppearance? Logbook { get; set; }
       
        public int? DataPanelSpecificationId { get; set; }
        public DataPanelSpecification? DataPanelSpecification { get; set; }


    }
}
