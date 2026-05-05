namespace GestionEnfermeria.DTO.Consultas
{
    public class DisponibilidadCampoDTO
    {
        public string CodigoCampo { get; set; }
        public int CapacidadTotal { get; set; }
        public int Ocupado { get; set; }
        public int Disponible { get; set; }
    }
}
