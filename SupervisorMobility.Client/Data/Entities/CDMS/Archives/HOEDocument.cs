namespace SupervisorMobility.Client.Data.Entities.CDMS.Documents
{
    public class HOEDocument
    {
        public int ID_DOC { get; set; }
        public int ID_Contribuidor { get; set; }
        public string Nombre { get; set; }
        public string Extension { get; set; }
        public string No_Rev { get; set; }
        public string Fecha_Carga { get; set; }
        public int Estatus { get; set; }
        public int ID_Carpeta { get; set; }
        public int ID_Revisor1 { get; set; }
        public int ID_Revisor2 { get; set; }
        public int ID_Revisor3 { get; set; }
        public string HOE_Code { get; set; }
        public string Document_Type_Hoe { get; set; }
        public string URL { get; set; }
        public bool Directory { get; set; }
    }
}
