using System.ComponentModel.DataAnnotations;

namespace GestionEnfermeria.Dominio
{
    public class Seguimiento
    {
        [Key]
        public int Id_Seguimiento { get; set; }
        public string Codigo_Seguro { get; set; }
        public string Codigo_Seguimiento { get; set; }
        public string Estado_Seguimiento { get; set; }
        public DateOnly Fecha_Inicio { get; set; }
        public DateOnly Fecha_Final { get; set; }
        public string Estado { get; set; } = "Activo";
        public List<Detalle_Seguimiento> Detalle_Seguimiento { get; set; }
    }
}
