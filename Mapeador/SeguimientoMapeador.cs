using GestionEnfermeria.Dominio;
using GestionEnfermeria.DTO;

namespace GestionEnfermeria.Mapeador
{
    public static class SeguimientoMapeador
    {
        public static SeguimientoDTO toSeguimientoDTO(this Seguimiento seguimiento)
        {
            return new SeguimientoDTO()
            {
                Codigo_Seguro = seguimiento.Codigo_Seguro,
                Codigo_Seguimiento = seguimiento.Codigo_Seguimiento,
                Estado_Seguimiento = seguimiento.Estado_Seguimiento,
                Fecha_Inicio = seguimiento.Fecha_Inicio,
                Fecha_Final = seguimiento.Fecha_Final
            };
        }
    }
}
