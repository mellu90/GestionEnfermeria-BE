using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionEnfermeria.Data;
using GestionEnfermeria.Dominio;
using System.Collections.Immutable;
using GestionEnfermeria.DTO;
using GestionEnfermeria.Mapeador;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GestionEnfermeria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnfermerasController : ControllerBase
    {
        private readonly GestionEnfermeriaContext _context;

        public EnfermerasController(GestionEnfermeriaContext context)
        {
            _context = context;
        }

        // GET: api/Enfermeras
        [HttpGet]
        public async Task<List<EnfermeraDTO>> GetEnfermera()
        {
            var enfermera = await (from a in _context.Enfermera
                                   where a.Estado == "Activo"
                                   select a).Select(al=>al.toEnfermeraDTO()).ToListAsync();
            return enfermera;
        }

        // GET: api/Enfermeras/5
        [HttpGet("{Codigo}")]
        public async Task<EnfermeraDTO> GetEnfermera(string Codigo)
        {
            var enfermera = await (from a in _context.Enfermera
                                   where a.Codigo_Enfermera == Codigo
                                   select a).FirstOrDefaultAsync();
            if (enfermera == null)
            {
                throw new Exception("La enfermera no existe");
            }
            return enfermera.toEnfermeraDTO();
        }

        // PUT: api/Enfermeras/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{Codigo}")]
        public async Task<EnfermeraDTO> PutEnfermera(string Codigo, [FromBody] EnfermeraDTO dto)
        {
            Enfermera enfermera = await (from e in _context.Enfermera
                                         where e.Codigo_Enfermera == Codigo && e.Estado == "Activo"
                                         select e).FirstOrDefaultAsync();
            if(enfermera == null)
            {
                throw new Exception("No se encontro a la enfermera");
            }
            dto.Codigo_Enfermera = dto.Codigo_Enfermera.Trim().ToUpper();

            enfermera.Codigo_Enfermera = dto.Codigo_Enfermera;
            enfermera.Nombre = dto.Nombre;
            enfermera.Apellido_Paterno = dto.Apellido_Paterno;
            enfermera.Apellido_Materno = dto.Apellido_Materno;
            
            _context.Enfermera.Update(enfermera);
            await _context.SaveChangesAsync();

            return enfermera.toEnfermeraDTO();
            
        }

        // POST: api/Enfermeras
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<EnfermeraDTO> PostEnfermera([FromBody] EnfermeraDTO dto)
        {
            var enfermeraExiste = await (from e in _context.Enfermera
                                         where dto.Codigo_Enfermera == e.Codigo_Enfermera
                                         select e).FirstOrDefaultAsync();
            if(enfermeraExiste != null)
            {
                throw new Exception("Esta enfermera ya existe");
            }
            dto.Codigo_Enfermera = dto.Codigo_Enfermera.Trim().ToUpper();
            Enfermera enfermera = new Enfermera
            {
                Codigo_Enfermera = dto.Codigo_Enfermera,
                Nombre = dto.Nombre,
                Apellido_Paterno = dto.Apellido_Paterno,
                Apellido_Materno = dto.Apellido_Materno,
                Estado = "Activo"
            };
            _context.Enfermera.Add(enfermera);
            await _context.SaveChangesAsync();

            return enfermera.toEnfermeraDTO();
        }

        // DELETE: api/Enfermeras/5
        [HttpDelete("{Codigo}")]
        public async Task<EnfermeraDTO> DeleteEnfermera(string Codigo)
        {
            var enfermera = await (from e in _context.Enfermera
                                   where e.Codigo_Enfermera == Codigo && e.Estado == "Activo"
                                   select e).FirstOrDefaultAsync();
            if(enfermera == null)
            {
                throw new Exception("No existe la enfermera");
            }

            var tieneAsignaciones = await(from a in _context.Asignar
                                    where a.Id_Enfermera == enfermera.Id_Enfermera && a.Estado == "Activo"
                                    select a).FirstOrDefaultAsync();
            if (tieneAsignaciones != null)
                throw new Exception("No se puede eliminar, tiene asignaciones activas.");
            var tieneSeguimientos = await (from ds in _context.Detalle_Seguimiento
                                    where ds.Id_Enfermera == enfermera.Id_Enfermera && ds.Estado == "Activo"
                                    select ds).FirstOrDefaultAsync();

            if (tieneSeguimientos != null)
                throw new Exception("No se puede eliminar, tiene seguimientos activos.");

            enfermera.Estado = "Inactivo";
            _context.Enfermera.Update(enfermera);
            await _context.SaveChangesAsync();
            return enfermera.toEnfermeraDTO();
        }

        
    }
}
