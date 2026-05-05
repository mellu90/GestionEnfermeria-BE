using GestionEnfermeria.Dominio;
using GestionEnfermeria.DTO;

namespace GestionEnfermeria.Mapeador
{
    public static class EnfermeraMapeador
    {
        public static EnfermeraDTO toEnfermeraDTO(this Enfermera enfermera)
        {
            return new EnfermeraDTO()
            {
                Codigo_Enfermera = enfermera.Codigo_Enfermera,
                Nombre = enfermera.Nombre,
                Apellido_Paterno = enfermera.Apellido_Paterno,
                Apellido_Materno = enfermera.Apellido_Materno
            };
        }
    }
}
