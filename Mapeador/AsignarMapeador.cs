using GestionEnfermeria.Dominio;
using GestionEnfermeria.DTO;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GestionEnfermeria.Mapeador
{
    public static class AsignarMapeador
    {
        public static async Task<AsignarDTO> toAsignarDTO(this Asignar asignar)
        {
            
            return new AsignarDTO()
            {
                
            };
        }
    }
}
