using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GestionEnfermeria.Dominio
{
    public class Asignar
    {
        [Key]
        public int Id_Asignar { get; set; }
        public int Id_Enfermera { get; set; }        
        public int Id_Turno { get; set; }
        public string Estado { get; set; } = "Activo";
        [ForeignKey("Id_Enfermera")]
        [JsonIgnore]
        public Enfermera Enfermera { get; set; }
        [ForeignKey("Id_Turno")]
        [JsonIgnore]
        public Turno Turno { get; set; }
    }
}
