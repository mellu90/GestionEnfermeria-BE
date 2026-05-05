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
            var asignar = await (from a in _context.Asignar
                                 join t in _context.Turno on a.Id_Turno equals t.Id_Turno
                                 join e in _context.Enfermera on a.Id_Enfermera equals e.Id_Enfermera
                                 where a.Estado == "Activo"
                                 select new
                                 {
                                     CodigoTurno = t.Codigo_Turno,
                                     CodigoEnfermera = e.Codigo_Enfermera
                                 }).ToListAsync();
            return Ok(asignar);
        }

        // GET: api/Asignar/5
        [HttpGet("{Codigo_Turno}/{Codigo_Enfermera}")]
        public async Task<IActionResult> GetAsignar(string Codigo_Turno, string Codigo_Enfermera)
        {
            var asignar = await(from a in _context.Asignar
                            join t in _context.Turno on a.Id_Turno equals t.Id_Turno
                            join e in _context.Enfermera on a.Id_Enfermera equals e.Id_Enfermera
                            where a.Estado == "Activo" && e.Codigo_Enfermera == Codigo_Enfermera && t.Codigo_Turno == Codigo_Turno
                            select new
                            {
                                CodigoEnfermera = e.Codigo_Enfermera,
                                CodigoTurno = t.Codigo_Turno
                            }).FirstOrDefaultAsync();
            return Ok(asignar);
        }

        // PUT: api/Asignar/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{Codigo_Turno}/{Codigo_Enfermera}")]
        public async Task<IActionResult> PutAsignar(string Codigo_Turno, string Codigo_Enfermera, string Nuevo_Codigo_Turno, string Nuevo_Codigo_Enfermera)
        {
            var asignar = await (from a in _context.Asignar
                                 where a.Turno.Codigo_Turno == Codigo_Turno && a.Enfermera.Codigo_Enfermera == Codigo_Enfermera && a.Estado == "Activo"
                                 select a)
                                .Include(a => a.Turno)
                                .Include(a => a.Enfermera)
                                .FirstOrDefaultAsync();
            if(asignar == null)
            {
                return BadRequest("No existe la relacion entre el turno y la enfermera.");
            }
            var TurnoExiste = await (from t in _context.Turno
                                     where t.Codigo_Turno == Nuevo_Codigo_Turno && t.Estado == "Activo"
                                     select t).FirstOrDefaultAsync();
            var EnfermeraExiste = await (from e in _context.Enfermera
                                         where e.Codigo_Enfermera == Nuevo_Codigo_Enfermera && e.Estado == "Activo"
                                         select e).FirstOrDefaultAsync();
            if(TurnoExiste == null || EnfermeraExiste == null)
            {
                return BadRequest("El turno o la enfermera no existe.");
            }

            asignar.Id_Turno = TurnoExiste.Id_Turno;
            asignar.Id_Enfermera = EnfermeraExiste.Id_Enfermera;
            _context.Asignar.Update(asignar);
            await _context.SaveChangesAsync();

            return Ok("Actualizado correctamente.");
        }

        // POST: api/Asignar
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostAsignar(string Codigo_Turno, string Codigo_Enfermera)
        {
            var asignarExiste = await (from a in _context.Asignar
                                       where a.Turno.Codigo_Turno == Codigo_Turno && a.Enfermera.Codigo_Enfermera == Codigo_Enfermera && a.Estado == "Activo"
                                       select a)
                                .Include(a => a.Turno)
                                .Include(a => a.Enfermera)
                                .FirstOrDefaultAsync();
            if (asignarExiste != null)
            {
                return BadRequest("Ya existe la relacion entre la enfermera y el turno.");
            }
            var TurnoExiste = await (from t in _context.Turno
                                     where t.Codigo_Turno == Codigo_Turno && t.Estado == "Activo"
                                     select t).FirstOrDefaultAsync();
            var EnfermeraExiste = await (from e in _context.Enfermera
                                         where e.Codigo_Enfermera == Codigo_Enfermera && e.Estado == "Activo"
                                         select e).FirstOrDefaultAsync();
            if (TurnoExiste == null || EnfermeraExiste == null)
            {
                return BadRequest("El turno o la enfermera no existe.");
            }
            
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

            Asignar asignar = new Asignar
            {
                Id_Turno = TurnoExiste.Id_Turno,
                Id_Enfermera = EnfermeraExiste.Id_Enfermera,
                Estado = "Activo"
            };
            _context.Asignar.Add(asignar);
            await _context.SaveChangesAsync();

            return Ok(new {
                        TurnoExiste.Codigo_Turno, 
                        EnfermeraExiste.Codigo_Enfermera
                  });
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
