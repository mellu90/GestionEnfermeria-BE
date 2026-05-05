using GestionEnfermeria.Dominio;
using GestionEnfermeria.DTO;

namespace GestionEnfermeria.Mapeador
{
    public static class CampoMapeador
    {
        public static CampoDTO toCampoDTO(this Campo campo)
        {
            return new CampoDTO()
            {
                Codigo_Campo = campo.Codigo_Campo,
                Cantidad = campo.Cantidad
            };
        }
    }
}
