using GestionEnfermeria.Dominio;
using GestionEnfermeria.DTO;

namespace GestionEnfermeria.Mapeador
{
    public static class TurnoMapeador
    {
        public static TurnoDTO toTurnoDTO(this Turno turno)
        {
            return new TurnoDTO()
            {
                Codigo_Turno = turno.Codigo_Turno,
                Nombre_Turno = turno.Nombre_Turno,
                Hora_Inicio = turno.Hora_Inicio,
                Hora_Final = turno.Hora_Final
            };
        }
    }
}
