namespace GestionEnfermeria.DTO
{
    public class SeguimientoDTO
    {
        public string Codigo_Seguro { get; set; }
        public string Codigo_Seguimiento { get; set; }
        public string Estado_Seguimiento { get; set; }
        public DateOnly Fecha_Inicio { get; set; }
        public DateOnly Fecha_Final { get; set; }
    }
}
