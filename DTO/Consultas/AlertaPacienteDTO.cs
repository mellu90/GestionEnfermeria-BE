namespace GestionEnfermeria.DTO.Consultas
{
    public class AlertaPacienteDTO
    {
        public string CodigoPaciente { get; set; }
        public string ObservacionCritica { get; set; }
        public DateOnly Fecha { get; set; }
    }
}
