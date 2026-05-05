namespace GestionEnfermeria.DTO.Consultas
{
    public class NotasAnterioresDTO
    {
        public string Codigo { get; set; }
        public string NombreEnfermera { get; set; }
        public string Nota { get; set; }
        public DateOnly Fecha_Inicio { get; set; }
        public DateOnly? Fecha_Final { get; set; }
    }
}
