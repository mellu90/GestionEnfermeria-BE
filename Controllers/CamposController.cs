using GestionEnfermeria.Data;
using GestionEnfermeria.Dominio;
using GestionEnfermeria.DTO;
using GestionEnfermeria.Mapeador;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace GestionEnfermeria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CamposController : ControllerBase
    {
        private readonly GestionEnfermeriaContext _context;

        public CamposController(GestionEnfermeriaContext context)
        {
            _context = context;
        }

        // GET: api/Campos
        [HttpGet]
        public async Task<List<CampoDTO>> GetCampo()
        {
            var campo = await (from c in _context.Campo
                               where c.Estado == "Activo"
                               select c).Select(ca => ca.toCampoDTO()).ToListAsync();
            return campo;
        }

        // GET: api/Campos/5
        [HttpGet("{Codigo_Campo}")]
        public async Task<CampoDTO> GetCampo(string Codigo_Campo)
        {
            return await (from c in _context.Campo
                          where c.Codigo_Campo == Codigo_Campo && c.Estado == "Activo"
                          select c.toCampoDTO()).FirstOrDefaultAsync();
        }

        // PUT: api/Campos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{Codigo_Campo}")]
        public async Task<CampoDTO> PutCampo(string Codigo_Campo, [FromBody] CampoDTO dto)
        {
            var campo = await (from c in _context.Campo
                               where c.Codigo_Campo == Codigo_Campo && c.Estado=="Activo"
                               select c).FirstOrDefaultAsync();
            if(campo == null)
            {
                throw new Exception("El campo no existe.");
            }
            dto.Codigo_Campo = dto.Codigo_Campo.Trim().ToUpper();
            campo.Codigo_Campo = dto.Codigo_Campo;
            campo.Cantidad = dto.Cantidad;
            _context.Campo.Update(campo);
            await _context.SaveChangesAsync();

            return campo.toCampoDTO();
        }

        // POST: api/Campos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<CampoDTO> PostCampo([FromBody] CampoDTO campoDTO)
        {
            var campoExiste = await (from c in _context.Campo
                                     where c.Codigo_Campo == campoDTO.Codigo_Campo && c.Estado == "Activo"
                                     select c).FirstOrDefaultAsync();
            if(campoExiste != null)
            {
                throw new Exception("Este campo ya existe.");
            }
            campoDTO.Codigo_Campo = campoDTO.Codigo_Campo.Trim().ToUpper();
            Campo campo = new Campo
            {
                Codigo_Campo = campoDTO.Codigo_Campo,
                Cantidad = campoDTO.Cantidad
            };
            _context.Campo.Add(campo);
            await _context.SaveChangesAsync();
            return campo.toCampoDTO();
        }

        // DELETE: api/Campos/5
        [HttpDelete("{Codigo_Campo}")]
        public async Task<CampoDTO> DeleteCampo(string Codigo_Campo)
        {
            var campo = await (from c in _context.Campo
                               where c.Codigo_Campo == Codigo_Campo && c.Estado == "Activo"
                               select c).FirstOrDefaultAsync();
            if(campo == null)
            {
                throw new Exception("No existe ningun campo con ese codigo.");
            }
            var tieneTurnos = await (from tc in _context.Turno_Campo
                            where tc.Id_Campo == campo.Id_Campo && tc.Estado == "Activo"
                            select tc).FirstOrDefaultAsync();

            if (tieneTurnos != null)
                throw new Exception("No se puede eliminar, está en uso en turnos.");
            
            campo.Estado = "Inactivo";
            _context.Campo.Update(campo);
            await _context.SaveChangesAsync();
            return campo.toCampoDTO();
        }
        
    }
}
