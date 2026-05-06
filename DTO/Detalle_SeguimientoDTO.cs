namespace GestionEnfermeria.DTO
{
    public class Detalle_SeguimientoDTO
    {
        public string Codigo_Seguimiento { get; set; }
        public string Codigo_Enfermera { get; set; }
        public string Codigo_Receta { get; set; }
        public string? Observacion { get; set; }
        public DateOnly Fecha_Inicio { get; set; }
        public DateOnly Fecha_Final { get; set; }
    }
}