using System.ComponentModel.DataAnnotations;

namespace GestionEnfermeria.Dominio
{
    public class Turno
    {
        [Key]
        public int Id_Turno { get; set; }
        public string Codigo_Turno { get; set; }
        public string Nombre_Turno { get; set; }
        public TimeOnly Hora_Inicio { get; set; }
        public TimeOnly Hora_Final { get; set; }
        public string Estado { get; set; } = "Activo";
        public List<Asignar> Asignar { get; set; }
        public List<Turno_Campo> Turno_Campo { get; set; }
    }
}
