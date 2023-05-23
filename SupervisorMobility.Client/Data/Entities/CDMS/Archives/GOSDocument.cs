namespace SupervisorMobility.Client.Data.Entities.CDMS.Documents
{
    public class GOSDocument
    {
        public int ID_DOC { get; set; }
        public int ID_Contribuidor { get; set; }
        public string Nombre { get; set;}
        public string Extension { get; set; }
        public string No_Rev { get; set; }
        public string Fecha_Carga { get; set; }
        public int Estatus { get; set; }
        public string GOS_Code { get; set; }
        public int ID_Aprobador { get; set; }
        public int ID_Revisor { get; set; }
        public string Folio_4Ms { get; set; }
        public string Key_Word { get; set; }
        private object _modelYear;
        public object Model_Year
        {
            get { return _modelYear; }
            set
            {
                if (value is int || value is string)
                {
                    _modelYear = value;
                }
            }
        }
        public string Fecha_Modificacion { get; set; }
        public int ID_UserModifico { get; set; }
        public string WFInstance { get; set; }
        public string URL { get; set; }
        public bool Directory { get; set; }
    }
}
