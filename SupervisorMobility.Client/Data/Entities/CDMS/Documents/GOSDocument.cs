namespace SupervisorMobility.Client.Data.Entities.CDMS.Documents
{
    public class GOSDocument
    {
        public int ID_DOC { get; }
        public int ID_Contribuidor { get; }
        public string Nombre { get; }
        public string Extension { get; }
        public string No_Rev { get; }
        public string Fecha_Carga { get; }
        public int Estatus { get; }
        public string GOS_Code { get; }
        public int ID_Aprobador { get; }
        public int ID_Revisor { get; }
        public string Folio_4Ms { get; }
        public string Key_Word { get; }
        public string Model_Year { get; }
        public string Fecha_Modificacion { get; }
        public int ID_UserModifico { get; }
        public string WFInstance { get; }
        public string URL { get; }
        public bool Directory { get; }
    }
}
