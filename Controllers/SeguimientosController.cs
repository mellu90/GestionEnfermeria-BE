using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionEnfermeria.Data;
using GestionEnfermeria.Dominio;
using GestionEnfermeria.Mapeador;
using GestionEnfermeria.DTO;

namespace GestionEnfermeria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeguimientosController : ControllerBase
    {
        private readonly GestionEnfermeriaContext _context;

        public SeguimientosController(GestionEnfermeriaContext context)
        {
            _context = context;
        }

        // GET: api/Seguimientos
        [HttpGet]
        public async Task<List<SeguimientoDTO>> GetSeguimiento()
        {
            var seguimiento = await(from s in _context.Seguimiento
                                    where s.Estado != "Inactivo"
                                    select s).Select(sa => sa.toSeguimientoDTO()).ToListAsync();
            return seguimiento;
        }

        // GET: api/Seguimientos/5
        [HttpGet("{Codigo}")]
        public async Task<SeguimientoDTO> GetSeguimiento(string Codigo)
        {
            var seguimiento = await(from s in _context.Seguimiento
                                    where s.Codigo_Seguimiento == Codigo
                                    select s).FirstOrDefaultAsync();

            if (seguimiento == null)
            {
                throw new Exception("El seguimiento no existe.");
            }

            return seguimiento.toSeguimientoDTO();
        }

        // PUT: api/Seguimientos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{Codigo_Seguimiento}")]
        public async Task<SeguimientoDTO> PutSeguimiento(string Codigo_Seguimiento, [FromBody] SeguimientoDTO dto)
        {
            var seguimiento = await (from s in _context.Seguimiento
                                           where s.Codigo_Seguimiento == Codigo_Seguimiento && s.Estado == "Activo"
                                           select s).FirstOrDefaultAsync();
            if (seguimiento == null)
                throw new Exception("No existe este seguimiento.");

            dto.Codigo_Seguimiento = dto.Codigo_Seguimiento.Trim().ToUpper();
            dto.Codigo_Seguro = dto.Codigo_Seguro.Trim().ToUpper();

            seguimiento.Codigo_Seguro = dto.Codigo_Seguro;
            seguimiento.Codigo_Seguimiento = dto.Codigo_Seguimiento;
            seguimiento.Estado_Seguimiento = dto.Estado_Seguimiento;
            seguimiento.Fecha_Inicio = dto.Fecha_Inicio;
            seguimiento.Fecha_Final = dto.Fecha_Final;

            _context.Seguimiento.Update(seguimiento);
            await _context.SaveChangesAsync();

            return seguimiento.toSeguimientoDTO();
        }

        // POST: api/Seguimientos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<SeguimientoDTO> PostSeguimiento([FromBody] SeguimientoDTO dto)
        {
            var seguimientoExiste = await (from s in _context.Seguimiento
                                           where s.Codigo_Seguimiento == dto.Codigo_Seguimiento
                                           select s).FirstOrDefaultAsync();
            if (seguimientoExiste != null)
                throw new Exception("Ya existe este seguimiento.");

            dto.Codigo_Seguimiento = dto.Codigo_Seguimiento.Trim().ToUpper();
            dto.Codigo_Seguro = dto.Codigo_Seguro.Trim().ToUpper();

            Seguimiento seguimiento = new Seguimiento
            {
                Codigo_Seguro = dto.Codigo_Seguro,
                Codigo_Seguimiento = dto.Codigo_Seguimiento,
                Estado_Seguimiento = dto.Estado_Seguimiento,
                Fecha_Inicio = dto.Fecha_Inicio,
                Fecha_Final = dto.Fecha_Final,
                Estado = "Activo"
            };
            _context.Seguimiento.Add(seguimiento);
            await _context.SaveChangesAsync();

            return seguimiento.toSeguimientoDTO();
        }

        // DELETE: api/Seguimientos/5
        [HttpDelete("{Codigo}")]
        public async Task<SeguimientoDTO> DeleteSeguimiento(string Codigo)
        {
            var seguimiento = await (from s in _context.Seguimiento
                                     where s.Codigo_Seguimiento == Codigo && s.Estado == "Activo"
                                     select s).FirstOrDefaultAsync();
            if (seguimiento == null)
                throw new Exception("No existe este seguimiento");
            var tieneDetalles = await (from ds in _context.Detalle_Seguimiento
                                where ds.Id_Seguimiento == seguimiento.Id_Seguimiento && ds.Estado == "Activo"
                                select ds).FirstOrDefaultAsync();

            if (tieneDetalles != null)
                throw new Exception("No se puede eliminar, tiene detalles activos.");
            seguimiento.Estado = "Inactivo";
            _context.Seguimiento.Update(seguimiento);
            await _context.SaveChangesAsync();

            return seguimiento.toSeguimientoDTO();
        }
    }
}
