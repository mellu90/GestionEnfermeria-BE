using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GestionEnfermeria.Dominio;

namespace GestionEnfermeria.Data
{
    public class GestionEnfermeriaContext : DbContext
    {
        public GestionEnfermeriaContext (DbContextOptions<GestionEnfermeriaContext> options)
            : base(options)
        {
        }

        public DbSet<GestionEnfermeria.Dominio.Enfermera> Enfermera { get; set; } = default!;
        public DbSet<GestionEnfermeria.Dominio.Seguimiento> Seguimiento { get; set; } = default!;
        public DbSet<GestionEnfermeria.Dominio.Detalle_Seguimiento> Detalle_Seguimiento { get; set; } = default!;
        public DbSet<GestionEnfermeria.Dominio.Asignar> Asignar { get; set; } = default!;
        public DbSet<GestionEnfermeria.Dominio.Turno> Turno { get; set; } = default!;
        public DbSet<GestionEnfermeria.Dominio.Turno_Campo> Turno_Campo { get; set; } = default!;
        public DbSet<GestionEnfermeria.Dominio.Campo> Campo { get; set; } = default!;
    }
}
