namespace GestionEnfermeria.DTO
{
    public class RecetaGetDTO
    {
        public string codigo { get; set; }
        public string pacienteCodigo { get; set; }
        public string pacienteNombre { get; set; }
        public string medicoCodigo { get; set; }
        public string medicoNombre { get; set; }
        public string fechaSolicitada { get; set; }
        public string estado { get; set; }
        public List<DetalleRecetaDTO> detalles { get; set; }
    }

    public class DetalleRecetaDTO
    {
        public string medicamentoCodigo { get; set; }
        public string medicamentoNombre { get; set; }
        public int cantidadSolicitada { get; set; }
        public string estado { get; set; }
        public PosologiaDTO posologia { get; set; }
    }

    public class PosologiaDTO
    {
        public string dosis { get; set; }
        public string viaAdministracion { get; set; }
        public string frecuencia { get; set; }
        public string duracion { get; set; }
        public string indicacionesAdicionales { get; set; }
    }
}