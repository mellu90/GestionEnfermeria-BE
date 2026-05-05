using GestionEnfermeria.Dominio;
using GestionEnfermeria.DTO;

namespace GestionEnfermeria.Mapeador
{
    public static class Turno_CampoMapeador
    {
        public static Turno_CampoDTO toTurnoCampoDTO(this Turno_Campo entity)
        {
            if (entity == null) return null;

            return new Turno_CampoDTO
            {
                Codigo_Turno = entity.Turno?.Codigo_Turno ?? "N/A",
                Codigo_Campo = entity.Campo?.Codigo_Campo ?? "N/A"
            };
        }
    }
}
