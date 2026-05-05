namespace GestionEnfermeria.DTO
{
    public class TurnoDTO
    {
        public string Codigo_Turno { get; set; }
        public string Nombre_Turno { get; set; }
        public TimeOnly Hora_Inicio { get; set; }
        public TimeOnly Hora_Final { get; set; }
    }
}
