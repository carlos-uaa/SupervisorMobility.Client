namespace SupervisorMobility.Client.Data.Entities
{
    public class OperationFiles
    {
        public int ID_DOC { get; set; }
        public int ID_Contribuidor { get; set; }
        public string Nombre { get; set; }
        public string Extension { get; set; }
        public int No_Rev { get; set; }
        public DateTime Fecha_Carga { get; set; }
        public bool Estatus { get; set; }
        public string CCP_Code { get; set; }
        public int ID_Aprobador { get; set; }
        public int ID_Revisor { get; set; }
        public string Folio_4Ms { get; set; }
        public string Key_Word { get; set; }
        public int Model_Year { get; set; }
        public DateTime Fecha_Modificacion { get; set; }
        public int ID_UserModifico { get; set; }
        public string WFInstance { get; set; }
        public string URL { get; set; }
        public string Directory { get; set; }
    }
}
