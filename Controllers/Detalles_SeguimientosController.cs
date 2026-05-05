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
            return Ok(await (from ds in _context.Detalle_Seguimiento
                             where ds.Estado != "Inactivo"
                             select new
                             {
                                 ds.Codigo_Receta,
                                 ds.Observacion,
                                 ds.Fecha_Inicio,
                                 ds.Fecha_Final
                             }).ToListAsync());
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
            var enfermera = await (from e in _context.Enfermera
                                             where e.Codigo_Enfermera == Codigo_Enfermera
                                             select e).FirstOrDefaultAsync();
            if (enfermera == null)
            {
                return BadRequest("No existe esa enfermera");
            }
            var seguimiento = await (from s in _context.Seguimiento
                                   where s.Codigo_Seguimiento == Codigo_Seguimiento
                                   select s).FirstOrDefaultAsync();
            if (seguimiento == null)
            {
                return BadRequest("No existe ese seguimiento");
            }
            var detalle_Seguimiento = await (from ds in _context.Detalle_Seguimiento
                                             where ds.Id_Enfermera == enfermera.Id_Enfermera && ds.Id_Seguimiento == seguimiento.Id_Seguimiento
                                             select ds).FirstOrDefaultAsync();
            if(detalle_Seguimiento == null)
            {
                return BadRequest("No existe la relacion entre la enfermera y el detalle");
            }

            return Ok(new
            {
                detalle_Seguimiento.Codigo_Receta,
                detalle_Seguimiento.Observacion,
                detalle_Seguimiento.Fecha_Inicio,
                detalle_Seguimiento.Fecha_Final
            });
        }

        // PUT: api/Detalles_Seguimientos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{Codigo_Enfermera}/{Codigo_Seguimiento}")]
        public async Task<IActionResult> PutDetalle_Seguimiento(string Codigo_Seguimiento, string Codigo_Enfermera, string Codigo_Receta, string Observacion, DateOnly Fecha_Inicio, DateOnly Fecha_Final)
        {
            var enfermera = await (from e in _context.Enfermera
                                   where e.Codigo_Enfermera == Codigo_Enfermera && e.Estado != "Inactivo"
                                   select e).FirstOrDefaultAsync();
            var seguimiento = await (from s in _context.Seguimiento
                                     where s.Codigo_Seguimiento == Codigo_Seguimiento && s.Estado != "Inactivo"
                                     select s).FirstOrDefaultAsync();
            if (enfermera == null || seguimiento == null)
                return BadRequest("La nueva enfermera o el seguimiento no existen.");
            var detalleExiste = await (from ds in _context.Detalle_Seguimiento
                                       where ds.Id_Enfermera == enfermera.Id_Enfermera && ds.Id_Seguimiento == seguimiento.Id_Seguimiento
                                       select ds).FirstOrDefaultAsync();

            detalleExiste.Codigo_Receta = Codigo_Receta.Trim().ToUpper();
            detalleExiste.Observacion = Observacion;            
            detalleExiste.Fecha_Inicio = Fecha_Inicio;
            detalleExiste.Fecha_Final = Fecha_Final;

            _context.Detalle_Seguimiento.Update(detalleExiste);
            await _context.SaveChangesAsync();

            return Ok("Actualizado correctamente");
        }

        // POST: api/Detalles_Seguimientos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostDetalle_Seguimiento(string Codigo_Seguimiento, string Codigo_Enfermera, string Codigo_Receta, string Observacion, DateOnly Fecha_Inicio, DateOnly Fecha_Final)
        {
            var enfermera = await (from e in _context.Enfermera
                                   where e.Codigo_Enfermera == Codigo_Enfermera && e.Estado == "Activo"
                                   select e).FirstOrDefaultAsync();
            var seguimiento = await (from s in _context.Seguimiento
                                     where s.Codigo_Seguimiento == Codigo_Seguimiento && s.Estado == "Activo"
                                     select s).FirstOrDefaultAsync();
            if (enfermera == null || seguimiento == null)
            {
                return BadRequest("Enfermera o Seguimiento no existe");
            }
            if (Fecha_Inicio > Fecha_Final)
                return BadRequest("La fecha de inicio no puede ser mayor a la fecha final.");
            if (Fecha_Inicio < seguimiento.Fecha_Inicio || Fecha_Final > seguimiento.Fecha_Final)
                return BadRequest("Las fechas están fuera del rango del seguimiento.");
            //Aqui ta la validacion de si la enfermera tiene un turno asignado o nao
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


            
            var detalle_seguimiento = new Detalle_Seguimiento
            {
                Id_Seguimiento = seguimiento.Id_Seguimiento,
                Id_Enfermera = enfermera.Id_Enfermera,
                Codigo_Receta = Codigo_Receta.Trim().ToUpper(),                
                Observacion = Observacion,                
                Fecha_Inicio = Fecha_Inicio,
                Fecha_Final = Fecha_Final,
                Estado = "Activo"
            };
            _context.Detalle_Seguimiento.Add(detalle_seguimiento);
            await _context.SaveChangesAsync();

            return Ok("Se creo exitosamente");
        }

        // DELETE: api/Detalles_Seguimientos/5
        [HttpDelete("borrar/{Codigo_Enfermera}/{Codigo_Seguimiento}")]
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
