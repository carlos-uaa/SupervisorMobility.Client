using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.Client.Data.Entities
{
    public class HeadCount
    {
        public int HeadCountId { get; set; }
        public int Codigo { get; set; }
        public string CO { get; set; }

        public int ID_Area { get; set; }
        public string Nombre_Area { get; set; }

        public int Cost_center { get; set; }
        public string ID_Departamento { get; set; }
        public string Nombre_Departamento { get; set; }

        public int ID_subarea { get; set; }
        public string nombre_subarea { get; set; }
        public string Fuction_Type { get; set; }

        public string Nivel { get; set; }
        public string Group { get; set; }
        public string BUDGET { get; set; }
        public string RTO { get; set; }
        public int HC { get; set; }
        public string? Comentarios { get; set; }
        public string LABOR_TYPE { get; set; }
        public DateTime Fecha_de_alta { get; set; }
        public string Usuario_de_alta { get; set; }
        public int? UserUploadId { get; set; }
    }
}
