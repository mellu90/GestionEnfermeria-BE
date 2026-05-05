using System.ComponentModel.DataAnnotations;

namespace GestionEnfermeria.Dominio
{
    public class Campo
    {
        [Key]
        public int Id_Campo { get; set; }
        public string Codigo_Campo { get; set; }
        public int Cantidad { get; set; }
        public string Estado { get; set; } = "Activo";
        public List<Turno_Campo> Turno_Campo { get; set; }
    }
}
