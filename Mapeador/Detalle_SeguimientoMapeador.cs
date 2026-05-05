using GestionEnfermeria.Dominio;
using GestionEnfermeria.DTO;

namespace GestionEnfermeria.Mapeador
{
    public static class Detalle_SeguimientoMapeador
    {
        public static Detalle_SeguimientoDTO toDetalle_SeguimientoDTO(this Detalle_Seguimiento entity)
        {
            if (entity == null) return null;

            return new Detalle_SeguimientoDTO()
            {
                Codigo_Seguimiento = entity.Seguimiento?.Codigo_Seguimiento ?? "N/A",
                Codigo_Enfermera = entity.Enfermera?.Codigo_Enfermera ?? "N/A",
                Codigo_Receta = entity.Codigo_Receta,
                Observacion = entity.Observacion,
                Fecha_Inicio = entity.Fecha_Inicio,
                Fecha_Final = entity.Fecha_Final
            };
        }
    }
}
