using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionEnfermeria.Data;
using GestionEnfermeria.Dominio;
using NuGet.Protocol;
using GestionEnfermeria.DTO;
using GestionEnfermeria.Mapeador;

namespace GestionEnfermeria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AsignarController : ControllerBase
    {
        private readonly GestionEnfermeriaContext _context;

        public AsignarController(GestionEnfermeriaContext context)
        {
            _context = context;
        }

        // GET: api/Asignar
        [HttpGet]
        public async Task<IActionResult> GetAsignar()
        {
                var asignar = await _context.Asignar
                .Include(a => a.Enfermera) // Carga los datos de la enfermera
                .Include(a => a.Turno)     // Carga los datos del turno
                .Where(a => a.Estado == "Activo")
                .ToListAsync();

            // Ahora mapeamos la lista de entidades a la lista de DTOs
            var asignarDTOs = asignar.Select(a => a.toAsignarDTO()).ToList();

            return Ok(asignarDTOs);
        }

        // GET: api/Asignar/5
        [HttpGet("{Codigo_Turno}/{Codigo_Enfermera}")]
        public async Task<IActionResult> GetAsignar(string Codigo_Turno, string Codigo_Enfermera)
        {
            var asignar = await _context.Asignar
                        .Include(a => a.Turno)
                        .Include(a => a.Enfermera)
                        .Where
                        (a => a.Estado == "Activo" &&
                         a.Enfermera.Codigo_Enfermera == Codigo_Enfermera &&
                         a.Turno.Codigo_Turno == Codigo_Turno)
                        .FirstOrDefaultAsync();

            if (asignar == null) return NotFound("No se encontró la asignación.");

            return Ok(asignar.toAsignarDTO());
        }

        // PUT: api/Asignar/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{Codigo_Turno}/{Codigo_Enfermera}")]
        public async Task<IActionResult> PutAsignar(string Codigo_Turno, string Codigo_Enfermera, [FromBody] AsignarDTO nuevoDto)
        {
            var asignar = await _context.Asignar
                        .Include(a => a.Turno)
                        .Include(a => a.Enfermera)
                        .FirstOrDefaultAsync(a => a.Turno.Codigo_Turno == Codigo_Turno &&
                                                 a.Enfermera.Codigo_Enfermera == Codigo_Enfermera &&
                                                 a.Estado == "Activo");
            if (asignar == null) return NotFound("No existe la relación original.");

            var nuevoT = await _context.Turno.FirstOrDefaultAsync(t => t.Codigo_Turno == nuevoDto.Codigo_Turno && t.Estado == "Activo");
            var nuevaE = await _context.Enfermera.FirstOrDefaultAsync(e => e.Codigo_Enfermera == nuevoDto.Codigo_Enfermera && e.Estado == "Activo");

            if (nuevoT == null || nuevaE == null) return BadRequest("Los nuevos códigos de turno o enfermera no son válidos.");

            asignar.Id_Turno = nuevoT.Id_Turno;
            asignar.Id_Enfermera = nuevaE.Id_Enfermera;

            _context.Entry(asignar).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(asignar.toAsignarDTO());
        }

        // POST: api/Asignar
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostAsignar([FromBody] AsignarDTO dto)
        {

            if (dto == null) return BadRequest("Datos inválidos.");
            var TurnoExiste = await _context.Turno.FirstOrDefaultAsync(t => t.Codigo_Turno == dto.Codigo_Turno && t.Estado == "Activo");
            var EnfermeraExiste = await _context.Enfermera.FirstOrDefaultAsync(e => e.Codigo_Enfermera == dto.Codigo_Enfermera && e.Estado == "Activo");

            if (TurnoExiste == null || EnfermeraExiste == null)
                return BadRequest("El turno o la enfermera no existen o están inactivos.");

            var turnoNuevo = TurnoExiste;

            var tieneConflicto = await (from a in _context.Asignar
                join t in _context.Turno on a.Id_Turno equals t.Id_Turno
                where a.Id_Enfermera == EnfermeraExiste.Id_Enfermera
                && a.Estado == "Activo"
                && (
                    turnoNuevo.Hora_Inicio < t.Hora_Final &&
                    turnoNuevo.Hora_Final > t.Hora_Inicio
                )
                select a).AnyAsync();

            if (tieneConflicto)
            {
                return BadRequest("La enfermera ya tiene un turno en ese horario.");
            }

            var turnoCampo = await (from tc in _context.Turno_Campo
                                    where tc.Id_Turno == TurnoExiste.Id_Turno
                                    && tc.Estado == "Activo"
                                    select tc)
                                    .FirstOrDefaultAsync();

            if (turnoCampo == null)
            {
                return BadRequest("El turno no tiene un campo asignado.");
            }

            
            var campo = await (from c in _context.Campo
                               where c.Id_Campo == turnoCampo.Id_Campo
                               && c.Estado == "Activo"
                               select c).FirstOrDefaultAsync();

            if (campo == null)
            {
                return BadRequest("El campo no existe o está inactivo.");
            }

            var cantidadAsignados = await _context.Asignar
                .CountAsync(a => a.Id_Turno == TurnoExiste.Id_Turno && a.Estado == "Activo");
            
            if (cantidadAsignados >= campo.Cantidad)
            {
                return BadRequest("Se alcanzó la capacidad máxima del campo.");
            }

            var nuevaAsignacion = new Asignar
            {
                Id_Turno = TurnoExiste.Id_Turno,
                Id_Enfermera = EnfermeraExiste.Id_Enfermera,
                Estado = "Activo"
            };

            _context.Asignar.Add(nuevaAsignacion);
            await _context.SaveChangesAsync();

            return Ok(nuevaAsignacion.toAsignarDTO());
        }
        

        // DELETE: api/Asignar/5
        [HttpDelete("{Codigo_Turno}/{Codigo_Enfermera}")]
        public async Task<IActionResult> DeleteAsignar(string Codigo_Turno, string Codigo_Enfermera)
        {
            var asignarExiste = await (from a in _context.Asignar
                                       where a.Turno.Codigo_Turno == Codigo_Turno && a.Enfermera.Codigo_Enfermera == Codigo_Enfermera && a.Estado == "Activo"
                                       select a)
                                .Include(a => a.Turno)
                                .Include(a => a.Enfermera)
                                .FirstOrDefaultAsync();
            if (asignarExiste == null)
            {
                return BadRequest("No existe la relacion entre la enfermera y el turno.");
            }

            asignarExiste.Estado = "Inactivo";
            _context.Asignar.Update(asignarExiste);
            await _context.SaveChangesAsync();

            return Ok("Eliminado correctamente");
        }

        
    }
}
