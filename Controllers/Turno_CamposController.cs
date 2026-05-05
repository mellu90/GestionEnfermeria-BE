using GestionEnfermeria.Data;
using GestionEnfermeria.Dominio;
using GestionEnfermeria.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestionEnfermeria.Mapeador;
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
            var registros = await _context.Turno_Campo
                            .Include(tc => tc.Turno)
                            .Include(tc => tc.Campo)
                            .Where(tc => tc.Estado == "Activo")
                            .ToListAsync();

            return Ok(registros.Select(tc => tc.toTurnoCampoDTO()));
        }

        // GET: api/Turno_Campos/5
        [HttpGet("{Codigo_Turno}/{Codigo_Campo}")]
        public async Task<IActionResult> GetTurno_Campo(string Codigo_Turno, string Codigo_Campo)
        {
            var registro = await _context.Turno_Campo
                            .Include(tc => tc.Turno)
                            .Include(tc => tc.Campo)
                            .FirstOrDefaultAsync(tc => tc.Turno.Codigo_Turno == Codigo_Turno &&
                                                     tc.Campo.Codigo_Campo == Codigo_Campo &&
                                                     tc.Estado == "Activo");

            if (registro == null) return NotFound("No existe la relación.");

            return Ok(registro.toTurnoCampoDTO());
        }

        // PUT: api/Turno_Campos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{Codigo_Turno}/{Codigo_Campo}")]
        public async Task<IActionResult> PutTurno_Campo(string Codigo_Turno, string Codigo_Campo, [FromBody] Turno_CampoDTO nuevoDto)
        {
            var registro = await _context.Turno_Campo
                .Include(tc => tc.Turno)
                .Include(tc => tc.Campo)
                .FirstOrDefaultAsync(tc => tc.Turno.Codigo_Turno == Codigo_Turno && tc.Campo.Codigo_Campo == Codigo_Campo && tc.Estado == "Activo");

            if (registro == null) return NotFound("Relación original no encontrada.");

            var nuevoT = await _context.Turno.FirstOrDefaultAsync(t => t.Codigo_Turno == nuevoDto.Codigo_Turno && t.Estado == "Activo");
            var nuevoC = await _context.Campo.FirstOrDefaultAsync(c => c.Codigo_Campo == nuevoDto.Codigo_Campo && c.Estado == "Activo");

            if (nuevoT == null || nuevoC == null) return BadRequest("Los nuevos códigos no son válidos.");

            registro.Id_Turno = nuevoT.Id_Turno;
            registro.Id_Campo = nuevoC.Id_Campo;

            _context.Entry(registro).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(registro.toTurnoCampoDTO());
        }

        // POST: api/Turno_Campos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostTurno_Campo([FromBody] Turno_CampoDTO dto)
        {
            if (dto == null) return BadRequest("Datos inválidos.");

            var turno = await _context.Turno.FirstOrDefaultAsync(t => t.Codigo_Turno == dto.Codigo_Turno && t.Estado == "Activo");
            var campo = await _context.Campo.FirstOrDefaultAsync(c => c.Codigo_Campo == dto.Codigo_Campo && c.Estado == "Activo");

            if (turno == null || campo == null) return BadRequest("Turno o Campo no encontrados.");

            // Validación de duplicado
            var existe = await _context.Turno_Campo.AnyAsync(tc => tc.Id_Turno == turno.Id_Turno && tc.Id_Campo == campo.Id_Campo && tc.Estado == "Activo");
            if (existe) return BadRequest("Esta relación ya existe.");

            var nuevo = new Turno_Campo
            {
                Id_Turno = turno.Id_Turno,
                Id_Campo = campo.Id_Campo,
                Estado = "Activo"
            };

            _context.Turno_Campo.Add(nuevo);
            await _context.SaveChangesAsync();

            return Ok(nuevo.toTurnoCampoDTO());
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
