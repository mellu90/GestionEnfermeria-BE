using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionEnfermeria.Data;
using GestionEnfermeria.Dominio;

namespace GestionEnfermeria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Turno_CamposController : ControllerBase
    {
        private readonly GestionEnfermeriaContext _context;

        public Turno_CamposController(GestionEnfermeriaContext context)
        {
            _context = context;
        }

        // GET: api/Turno_Campos
        [HttpGet]
        public async Task<IActionResult> GetTurno_Campo()
        {
            var turno_campo = await (from tc in _context.Turno_Campo
                                     join t in _context.Turno on tc.Id_Turno equals t.Id_Turno
                                     join c in _context.Campo on tc.Id_Campo equals c.Id_Campo
                                     where tc.Estado == "Activo"
                                     select new
                                     {
                                         CodigoTurno = t.Codigo_Turno,
                                         CodigoCampo = c.Codigo_Campo
                                     }).ToListAsync();
            return Ok(turno_campo);
        }

        // GET: api/Turno_Campos/5
        [HttpGet("{Codigo_Turno}/{Codigo_Campo}")]
        public async Task<IActionResult> GetTurno_Campo(string Codigo_Turno, string Codigo_Campo)
        {
            var turno_Campo = await (from tc in _context.Turno_Campo
                                     join t in _context.Turno on tc.Id_Turno equals t.Id_Turno
                                     join c in _context.Campo on tc.Id_Campo equals c.Id_Campo
                                     where tc.Estado == "Activo" && c.Codigo_Campo == Codigo_Campo && t.Codigo_Turno == Codigo_Turno
                                     select new {
                                         CodigoTurno = t.Codigo_Turno,
                                         CodigoCampo = c.Codigo_Campo
                                     }).FirstOrDefaultAsync();

            if (turno_Campo == null)
            {
                return BadRequest("No existe la relacion entre ese turno y ese campo");
            }

            return Ok(turno_Campo);
        }

        // PUT: api/Turno_Campos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{Codigo_Turno}/{Codigo_Campo}")]
        public async Task<IActionResult> PutTurno_Campo(string Codigo_Turno, string Codigo_Campo, string Nuevo_Codigo_Turno, string Nuevo_Codigo_Campo)
        {
            var turno_Campo = await (from tc in _context.Turno_Campo
                                     where tc.Turno.Codigo_Turno == Codigo_Turno && tc.Campo.Codigo_Campo == Codigo_Campo && tc.Estado == "Activo"
                                     select tc)
                                    .Include(a => a.Turno)
                                    .Include(a => a.Campo)
                                    .FirstOrDefaultAsync();
            if (turno_Campo == null)
            {
                return BadRequest("La relacion del turno con el campo no existe");
            }
            var turnoExiste = await (from t in _context.Turno
                                     where t.Codigo_Turno == Nuevo_Codigo_Turno && t.Estado == "Activo"
                                     select t).FirstOrDefaultAsync();
            var campoExiste = await (from c in _context.Campo
                                         where c.Codigo_Campo == Nuevo_Codigo_Campo && c.Estado == "Activo"
                                         select c).FirstOrDefaultAsync();
            if (turnoExiste == null || campoExiste == null)
            {
                return BadRequest("El turno o el campo no existe.");
            }
            var existeDuplicado = await _context.Turno_Campo.AnyAsync(tc =>
                                tc.Id_Turno == turnoExiste.Id_Turno &&
                                tc.Id_Campo == campoExiste.Id_Campo &&
                                tc.Estado == "Activo"
            );

            if (existeDuplicado)
                return BadRequest("Ya existe esta relación turno-campo.");            
            turno_Campo.Id_Turno = turnoExiste.Id_Turno;
            turno_Campo.Id_Campo = campoExiste.Id_Campo;

            _context.Turno_Campo.Update(turno_Campo);
            await _context.SaveChangesAsync();

            return Ok("Actualizado correctamente.");
            
        }

        // POST: api/Turno_Campos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostTurno_Campo(string Codigo_Turno, string Codigo_Campo)
        {
            var turno_CampoExiste = await (from tc in _context.Turno_Campo
                                     where tc.Turno.Codigo_Turno == Codigo_Turno && tc.Campo.Codigo_Campo == Codigo_Campo && tc.Estado == "Activo"
                                     select tc)
                                .Include(tc => tc.Turno)
                                .Include(tc => tc.Campo)
                                .FirstOrDefaultAsync();
            if (turno_CampoExiste != null)
            {
                return Ok("Ya existe la relacion entre el turno y campo");
            }
            var turnoExiste = await (from t in _context.Turno
                                     where t.Codigo_Turno == Codigo_Turno && t.Estado == "Activo"
                                     select t).FirstOrDefaultAsync();
            var campoExiste = await (from c in _context.Campo
                                     where c.Codigo_Campo == Codigo_Campo && c.Estado == "Activo"
                                     select c).FirstOrDefaultAsync();
            if (turnoExiste == null || campoExiste == null)
                return BadRequest("No existe el turno o el campo");

            Turno_Campo turno_campo = new Turno_Campo
            {
                Id_Turno = turnoExiste.Id_Turno,
                Id_Campo = campoExiste.Id_Campo
            };

            _context.Turno_Campo.Add(turno_campo);
            await _context.SaveChangesAsync();

            return Ok("Se creo exitosamente");
        }

        // DELETE: api/Turno_Campos/5
        [HttpDelete("{Codigo_Turno}/{Codigo_Campo}")]
        public async Task<IActionResult> DeleteTurno_Campo(string Codigo_Turno, string Codigo_Campo)
        {
            var turno_Campo = await (from tc in _context.Turno_Campo
                                     where tc.Turno.Codigo_Turno == Codigo_Turno && tc.Campo.Codigo_Campo == Codigo_Campo && tc.Estado == "Activo"
                                     select tc)
                                    .Include(a => a.Turno)
                                    .Include(a => a.Campo)
                                    .FirstOrDefaultAsync();
            if (turno_Campo == null)
            {
                return BadRequest("La relacion del turno con el campo no existe");
            }
            turno_Campo.Estado = "Inactivo";

            _context.Turno_Campo.Update(turno_Campo);
            await _context.SaveChangesAsync();

            return Ok("Eliminado con exito");
        }

    }
}
