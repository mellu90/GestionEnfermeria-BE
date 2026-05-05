namespace GestionEnfermeria.DTO.Consultas
{
    public class PacienteEgresoDTO
    {
        public string CodigoPaciente { get; set; }
        public DateOnly UltimoSeguimiento { get; set; }
        public string Motivo { get; set; }
    }
}
