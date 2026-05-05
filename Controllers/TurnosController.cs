using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionEnfermeria.Data;
using GestionEnfermeria.Dominio;
using GestionEnfermeria.DTO;
using GestionEnfermeria.Mapeador;

namespace GestionEnfermeria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TurnosController : ControllerBase
    {
        private readonly GestionEnfermeriaContext _context;

        public TurnosController(GestionEnfermeriaContext context)
        {
            _context = context;
        }

        // GET: api/Turnos
        [HttpGet]
        public async Task<List<TurnoDTO>> GetTurno()
        {
            var turno = await (from t in _context.Turno
                               where t.Estado == "Activo"
                               select t).Select(ta => ta.toTurnoDTO()).ToListAsync();
            return turno;
        }

        // GET: api/Turnos/5
        [HttpGet("{Codigo_Turno}")]
        public async Task<TurnoDTO> GetTurno(string Codigo_Turno)
        {
            return await(from t in _context.Turno
                         where t.Codigo_Turno == Codigo_Turno && t.Estado == "Activo"
                         select t.toTurnoDTO()).FirstOrDefaultAsync();
        }

        // PUT: api/Turnos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{Codigo_Turno}")]
        public async Task<TurnoDTO> PutTurno(string Codigo_Turno, [FromBody] TurnoDTO dto)
        {
            var turno = await (from t in _context.Turno
                               where t.Codigo_Turno == Codigo_Turno && t.Estado == "Activo"
                               select t).FirstOrDefaultAsync();
            if(turno == null)
            {
                throw new Exception("No existe el turno.");
            }
            if (dto.Hora_Inicio >= dto.Hora_Final)
                throw new Exception("La hora de inicio debe ser menor a la hora final.");

            var existeHorario = await _context.Turno.AnyAsync(t =>
                                t.Hora_Inicio == dto.Hora_Inicio &&
                                t.Hora_Final == dto.Hora_Final &&
                                t.Estado == "Activo");
            dto.Codigo_Turno = dto.Codigo_Turno.Trim().ToUpper();

            turno.Codigo_Turno = dto.Codigo_Turno;
            turno.Nombre_Turno = dto.Nombre_Turno;
            turno.Hora_Inicio = dto.Hora_Inicio;
            turno.Hora_Final = dto.Hora_Final;

            _context.Turno.Update(turno);
            await _context.SaveChangesAsync();

            return turno.toTurnoDTO();
        }

        // POST: api/Turnos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<TurnoDTO> PostTurno([FromBody] TurnoDTO dto)
        {
            var turnoExiste = await (from t in _context.Turno
                                     where t.Codigo_Turno == dto.Codigo_Turno
                                     select t).FirstOrDefaultAsync();
            if(turnoExiste != null)
            {
                throw new Exception("Este turno ya existe.");
            }
            if (dto.Hora_Inicio >= dto.Hora_Final)
                throw new Exception("La hora de inicio debe ser menor a la hora final.");

            var existeHorario = await _context.Turno.AnyAsync(t =>
                                t.Hora_Inicio == dto.Hora_Inicio &&
                                t.Hora_Final == dto.Hora_Final &&
                                t.Estado == "Activo");

            if (existeHorario)
                throw new Exception("Ya existe un turno con ese horario.");

            dto.Codigo_Turno = dto.Codigo_Turno.Trim().ToUpper();

            Turno turno = new Turno
            {
                Codigo_Turno = dto.Codigo_Turno,
                Nombre_Turno = dto.Nombre_Turno,
                Hora_Inicio = dto.Hora_Inicio,
                Hora_Final = dto.Hora_Final
            };

            _context.Turno.Add(turno);
            await _context.SaveChangesAsync();

            return turno.toTurnoDTO();

        }

        // DELETE: api/Turnos/5
        [HttpDelete("{Codigo_Turno}")]
        public async Task<TurnoDTO> DeleteTurno(string Codigo_Turno)
        {
            var turno = await (from t in _context.Turno
                               where t.Codigo_Turno == Codigo_Turno && t.Estado == "Activo"
                               select t).FirstOrDefaultAsync();
            if(turno == null)
            {
                throw new Exception("No existe este turno");
            }
            var tieneAsignaciones = await (from a in _context.Asignar
                                    where a.Id_Turno == turno.Id_Turno && a.Estado == "Activo"
                                    select a).FirstOrDefaultAsync();

            if (tieneAsignaciones != null)
                throw new Exception("No se puede eliminar, tiene enfermeras asignadas.");

            var tieneCampos = await (from tc in _context.Turno_Campo
                            where tc.Id_Turno == turno.Id_Turno && tc.Estado == "Activo"
                            select tc).FirstOrDefaultAsync();

            if (tieneCampos != null)
                throw new Exception("No se puede eliminar, está asociado a un campo.");

            turno.Estado = "Inactivo";
            _context.Turno.Update(turno);
            await _context.SaveChangesAsync();

            return turno.toTurnoDTO();
            
        }
    }
}
