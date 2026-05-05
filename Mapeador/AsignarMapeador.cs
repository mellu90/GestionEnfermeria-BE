using GestionEnfermeria.Dominio;
using GestionEnfermeria.DTO;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GestionEnfermeria.Mapeador
{
    public static class AsignarMapeador
    {
        public static AsignarDTO toAsignarDTO(this Asignar entity)
        {
            return new AsignarDTO()
            {
                // Accedemos a las propiedades de las tablas relacionadas
                Codigo_Enfermera = entity.Enfermera?.Codigo_Enfermera ?? "Sin Código",
                Codigo_Turno = entity.Turno?.Codigo_Turno ?? "Sin Turno"
            };
        }
    }
}
