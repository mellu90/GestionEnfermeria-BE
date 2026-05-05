namespace GestionEnfermeria.DTO.Consultas
{
    public class MedicinaPendienteDTO
    {
        public string Paciente { get; set; }
        public string Medicamento { get; set; }
        public string EnfermeraAsignada { get; set; }
        public DateOnly FechaInicio { get; set; }
    }
}
