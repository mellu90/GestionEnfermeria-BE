using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GestionEnfermeria.Dominio
{
    public class Turno_Campo
    {
        [Key]
        public int Id_Turno_Campo { get; set; }
        public int Id_Turno { get; set; }
        public int Id_Campo { get; set; }
        public string Estado { get; set; } = "Activo";
        [ForeignKey("Id_Turno")]
        [JsonIgnore]
        public Turno Turno { get; set; }
        [ForeignKey("Id_Campo")]
        [JsonIgnore]
        public Campo Campo { get; set; }
    }
}
