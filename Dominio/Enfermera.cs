using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GestionEnfermeria.Dominio
{
    public class Enfermera
    {
        [Key]
        public int Id_Enfermera { get; set; }
        public string Codigo_Enfermera { get; set; }
        public string Nombre {  get; set; }
        public string Apellido_Paterno {  get; set; }
        public string Apellido_Materno { get; set; }
        public string Estado { get; set; } = "Activo";
        public List<Detalle_Seguimiento> Detalle_Seguimiento { get; set; }
        public List<Asignar> Asignar { get; set; }
    }
}
