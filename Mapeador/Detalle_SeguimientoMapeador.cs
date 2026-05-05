using GestionEnfermeria.Dominio;
using GestionEnfermeria.DTO;

namespace GestionEnfermeria.Mapeador
{
    public static class Detalle_SeguimientoMapeador
    {
        public static Detalle_SeguimientoDTO toDetalle_SeguimientoDTO(this Detalle_Seguimiento detalle_seguimiento)
        {
            return new Detalle_SeguimientoDTO()
            {
                Codigo_Receta = detalle_seguimiento.Codigo_Receta,
                Observacion = detalle_seguimiento.Observacion,
                Fecha_Inicio = detalle_seguimiento.Fecha_Inicio,
                Fecha_Final = detalle_seguimiento.Fecha_Final
            };
        }
    }
}
