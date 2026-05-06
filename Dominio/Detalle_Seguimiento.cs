using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GestionEnfermeria.Dominio
{
    public class Detalle_Seguimiento
    {
        [Key]
        public int Id_Detalle_Seguimiento { get; set; }
        public int Id_Seguimiento { get; set; }
        public int Id_Enfermera { get; set; }
        public string Codigo_Receta { get; set; }
        public string? Observacion { get; set; }
        public DateOnly Fecha_Inicio { get; set; }
        public DateOnly Fecha_Final { get; set; }
        public string Estado { get; set; } = "Activo";
        [ForeignKey("Id_Enfermera")]
        [JsonIgnore]
        public Enfermera Enfermera { get; set; }
        [ForeignKey("Id_Seguimiento")]
        [JsonIgnore]
        public Seguimiento Seguimiento { get; set; }
    }
}
