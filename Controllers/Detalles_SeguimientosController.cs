using GestionEnfermeria.Data;
using GestionEnfermeria.Dominio;
using GestionEnfermeria.DTO;
using GestionEnfermeria.Mapeador;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestionEnfermeria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Detalles_SeguimientosController : ControllerBase
    {
        private readonly GestionEnfermeriaContext _context;

        public Detalles_SeguimientosController(GestionEnfermeriaContext context)
        {
            _context = context;
        }

        // GET: api/Detalles_Seguimientos
        [HttpGet]
        public async Task<IActionResult> GetDetalle_Seguimiento()
        {
            var detalles = await _context.Detalle_Seguimiento
                            .Include(d => d.Seguimiento)
                            .Include(d => d.Enfermera)
                            .Where(d => d.Estado == "Activo")
                            .ToListAsync();
                        
            var detallesDTO = detalles.Select(d => d.toDetalle_SeguimientoDTO()).ToList();

            return Ok(detallesDTO);
        }
        [HttpGet("ListadoPorEnfermera")]
        public async Task<IActionResult> GetListadoEnfermera()
        {
            var consulta = await (from ds in _context.Detalle_Seguimiento
                                  join e in _context.Enfermera on ds.Id_Enfermera equals e.Id_Enfermera
                                  where ds.Estado == "Activo"
                                  select new
                                  {
                                      Enfermera = e.Nombre + " " + e.Apellido_Paterno,
                                      ds.Codigo_Receta,
                                      ds.Observacion
                                  }).ToListAsync();
            return Ok(consulta);
        }
        [HttpGet("ReporteGeneral")]
        public async Task<IActionResult> GetReporteGeneral()
        {
            var consulta = await (from ds in _context.Detalle_Seguimiento
                                  join e in _context.Enfermera on ds.Id_Enfermera equals e.Id_Enfermera
                                  join s in _context.Seguimiento on ds.Id_Seguimiento equals s.Id_Seguimiento
                                  where ds.Estado == "Activo"
                                  select new
                                  {
                                      Enfermera = e.Nombre + " " + e.Apellido_Paterno,
                                      Seguimiento = s.Codigo_Seguimiento,
                                      EstadoPaciente = s.Estado_Seguimiento,
                                      ds.Observacion
                                  }).ToListAsync();
            return Ok(consulta);
        }

        // GET: api/Detalles_Seguimientos/5
        [HttpGet("{Codigo_Enfermera}/{Codigo_Seguimiento}")]
        public async Task<IActionResult> GetDetalle_Seguimiento(string Codigo_Enfermera, string Codigo_Seguimiento)
        {
            var detalle = await _context.Detalle_Seguimiento
            .Include(d => d.Enfermera)
            .Include(d => d.Seguimiento)
            .FirstOrDefaultAsync(d => d.Seguimiento.Codigo_Seguimiento == Codigo_Seguimiento &&
                                     d.Enfermera.Codigo_Enfermera == Codigo_Enfermera &&
                                     d.Estado == "Activo");

            if (detalle == null) return NotFound("No se encontró la relación de seguimiento para esta enfermera.");

            return Ok(detalle.toDetalle_SeguimientoDTO());
        }

        // PUT: api/Detalles_Seguimientos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{Codigo_Enfermera}/{Codigo_Seguimiento}")]
        public async Task<IActionResult> PutDetalle_Seguimiento(string Codigo_Seguimiento, string Codigo_Enfermera, [FromBody] Detalle_SeguimientoDTO nuevoDto)
        {
            var registro = await _context.Detalle_Seguimiento
            .Include(d => d.Enfermera)
            .Include(d => d.Seguimiento)
            .FirstOrDefaultAsync(d => d.Seguimiento.Codigo_Seguimiento == Codigo_Seguimiento &&
                                     d.Enfermera.Codigo_Enfermera == Codigo_Enfermera &&
                                     d.Estado == "Activo");

            if (registro == null) return NotFound("No existe el detalle original para actualizar.");

            
            var nuevaEnf = await _context.Enfermera.FirstOrDefaultAsync(e => e.Codigo_Enfermera == nuevoDto.Codigo_Enfermera && e.Estado == "Activo");
            var nuevoSeg = await _context.Seguimiento.FirstOrDefaultAsync(s => s.Codigo_Seguimiento == nuevoDto.Codigo_Seguimiento && s.Estado == "Activo");

            if (nuevaEnf == null || nuevoSeg == null) return BadRequest("Los nuevos códigos de enfermera o seguimiento no son válidos.");

            
            registro.Id_Enfermera = nuevaEnf.Id_Enfermera;
            registro.Id_Seguimiento = nuevoSeg.Id_Seguimiento;
            registro.Codigo_Receta = nuevoDto.Codigo_Receta;
            registro.Observacion = nuevoDto.Observacion;
            registro.Fecha_Inicio = nuevoDto.Fecha_Inicio;
            registro.Fecha_Final = nuevoDto.Fecha_Final;

            _context.Entry(registro).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(registro.toDetalle_SeguimientoDTO());
        }

        // POST: api/Detalles_Seguimientos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostDetalle_Seguimiento([FromBody] Detalle_SeguimientoDTO dto)
        {
            var enfermera = await (from e in _context.Enfermera
                                   where e.Codigo_Enfermera == dto.Codigo_Enfermera && e.Estado == "Activo"
                                   select e).FirstOrDefaultAsync();
            var seguimiento = await (from s in _context.Seguimiento
                                     where s.Codigo_Seguimiento == dto.Codigo_Seguimiento && s.Estado == "Activo"
                                     select s).FirstOrDefaultAsync();
            if (enfermera == null || seguimiento == null)
            {
                return BadRequest("Enfermera o Seguimiento no existe");
            }
            if (dto.Fecha_Inicio > dto.Fecha_Final)
                return BadRequest("La fecha de inicio no puede ser mayor a la fecha final.");
            if (dto.Fecha_Inicio < seguimiento.Fecha_Inicio || dto.Fecha_Final > seguimiento.Fecha_Final)
                return BadRequest("Las fechas están fuera del rango del seguimiento.");
            
            var estaAsignada = await _context.Asignar.AnyAsync(a =>
               a.Id_Enfermera == enfermera.Id_Enfermera &&
               a.Estado == "Activo");

            if (!estaAsignada)
                return BadRequest("La enfermera no tiene turnos asignados.");

            var detalle_Seguimiento_Existe = await (from ds in _context.Detalle_Seguimiento
                                                    where ds.Id_Enfermera == enfermera.Id_Enfermera && ds.Id_Seguimiento == seguimiento.Id_Seguimiento
                                                    select ds).FirstOrDefaultAsync();
            if (detalle_Seguimiento_Existe != null)
            {
                return BadRequest("La relacion entre el seguimiento y enfermera existe");
            }

            var nuevoDetalle = new Detalle_Seguimiento
            {
                Id_Seguimiento = seguimiento.Id_Seguimiento,
                Id_Enfermera = enfermera.Id_Enfermera,
                Codigo_Receta = dto.Codigo_Receta,
                Observacion = dto.Observacion,
                Fecha_Inicio = dto.Fecha_Inicio,
                Fecha_Final = dto.Fecha_Final,
                Estado = "Activo"
            };

            _context.Detalle_Seguimiento.Add(nuevoDetalle);
            await _context.SaveChangesAsync();

            return Ok(nuevoDetalle.toDetalle_SeguimientoDTO());
        }

        // DELETE: api/Detalles_Seguimientos/5
        [HttpDelete("{Codigo_Enfermera}/{Codigo_Seguimiento}")]
        public async Task<IActionResult> DeleteDetalle_Seguimiento(string Codigo_Enfermera, string Codigo_Seguimiento)
        {
            var detalle_Seguimiento = await (from ds in _context.Detalle_Seguimiento
                                             where ds.Enfermera.Codigo_Enfermera == Codigo_Enfermera && ds.Seguimiento.Codigo_Seguimiento == Codigo_Seguimiento
                                             select ds)
                                             .Include(ds => ds.Enfermera)
                                             .Include(ds => ds.Seguimiento)
                                             .FirstOrDefaultAsync();
            if (detalle_Seguimiento == null)
            {
                return BadRequest("No se pudo borrar");
            }
            detalle_Seguimiento.Estado = "Inactivo";
            _context.Detalle_Seguimiento.Update(detalle_Seguimiento);
            await _context.SaveChangesAsync();

            return Ok("Borrado exitosamente");
        }

    }
}
