using GestionEnfermeria.Data;
using GestionEnfermeria.DTO.Consultas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionEnfermeria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultasController : ControllerBase
    {
        private readonly GestionEnfermeriaContext _context;
        public ConsultasController(GestionEnfermeriaContext context)
        {
            _context = context;
        }
        [HttpGet("ConteoEnfermerasPorTurno")]
        public async Task<IActionResult> GetConteoEnfermeras()
        {
            var consulta = await (from a in _context.Asignar
                                  join t in _context.Turno on a.Id_Turno equals t.Id_Turno
                                  where a.Estado == "Activo"
                                  group a by t.Nombre_Turno into grupo
                                  select new EnfermerasPorTurnoDTO
                                  {
                                      Turno = grupo.Key,
                                      CantidadEnfermeras = grupo.Count()
                                  }).ToListAsync();
            return Ok(consulta);
        }
        [HttpGet("ListadoSeguimientosActivos")]
        public async Task<IActionResult> GetListadoGeneral()
        {
            var consulta = await (from ds in _context.Detalle_Seguimiento
                                  join e in _context.Enfermera on ds.Id_Enfermera equals e.Id_Enfermera
                                  join s in _context.Seguimiento on ds.Id_Seguimiento equals s.Id_Seguimiento
                                  where ds.Estado == "Activo" && e.Estado == "Activo"
                                  select new DetalleSeguimientoConsultaDTO
                                  {
                                      CodigoEnfermera = e.Codigo_Enfermera,
                                      Paciente = s.Codigo_Seguimiento,
                                      Receta = ds.Codigo_Receta,
                                      Observacion = ds.Observacion
                                  }).ToListAsync();
            return Ok(consulta);
        }
        [HttpGet("DisponibilidadRealCampos")]
        public async Task<IActionResult> GetDisponibilidadReal()
        {
            var consulta = await (from c in _context.Campo
                                  join tc in _context.Turno_Campo on c.Id_Campo equals tc.Id_Campo
                                  where c.Estado == "Activo" && tc.Estado == "Activo"
                                  select new DisponibilidadCampoDTO
                                  {
                                      CodigoCampo = c.Codigo_Campo,
                                      CapacidadTotal = Convert.ToInt32(c.Cantidad),                                      
                                      Ocupado = _context.Asignar.Count(a => a.Id_Turno == tc.Id_Turno && a.Estado == "Activo"),                                      
                                      Disponible = Convert.ToInt32(c.Cantidad) - _context.Asignar.Count(a => a.Id_Turno == tc.Id_Turno && a.Estado == "Activo")
                                  }).ToListAsync();

            return Ok(consulta);
        }
        [HttpGet("EnfermerasDisponibles")]
        public async Task<IActionResult> GetEnfermerasDisponibles()
        {
            const int LimiteTareas = 5;
            var hoy = DateOnly.FromDateTime(DateTime.Now);

            var consulta = await (from e in _context.Enfermera
                                  where e.Estado == "Activo"
                                  let tareasVigentes = _context.Detalle_Seguimiento.Count(ds =>
                                      ds.Id_Enfermera == e.Id_Enfermera &&
                                      ds.Estado == "Activo" && // Borrado lógico del detalle
                                      _context.Seguimiento.Any(s => s.Id_Seguimiento == ds.Id_Seguimiento
                                                                 && s.Estado_Seguimiento != "FINALIZADO") && // Validamos el Padre
                                      ds.Fecha_Final >= hoy)
                                  where tareasVigentes < LimiteTareas
                                  select new
                                  {
                                      e.Codigo_Enfermera,
                                      NombreCompleto = e.Nombre + " " + e.Apellido_Paterno,
                                      TareasVigentes = tareasVigentes,
                                      CuposLibres = LimiteTareas - tareasVigentes
                                  }).ToListAsync();

            return Ok(consulta);
        }
        [HttpGet("TurnosSobrepasanCapacidad")]
        public async Task<IActionResult> GetTurnosSobrepasanCapacidad()
        {
            var consulta = await (from a in _context.Asignar
                                  join t in _context.Turno on a.Id_Turno equals t.Id_Turno
                                  join tc in _context.Turno_Campo on t.Id_Turno equals tc.Id_Turno
                                  join c in _context.Campo on tc.Id_Campo equals c.Id_Campo
                                  where a.Estado == "Activo" && c.Estado == "Activo"
                                  group a by new { t.Nombre_Turno, c.Codigo_Campo, c.Cantidad } into grupo
                                  where grupo.Count() > Convert.ToInt32(grupo.Key.Cantidad)
                                  select new TurnosCriticosDTO
                                  {
                                      NombreTurno = grupo.Key.Nombre_Turno + " en " + grupo.Key.Codigo_Campo,
                                      TotalAsignados = grupo.Count(),
                                      Mensaje = "ALERTA: Se superó la capacidad máxima de " + grupo.Key.Cantidad + " personas."
                                  }).ToListAsync();

            return Ok(consulta);
        }
        //Revisar pacientes que ya se fueron
        [HttpGet("PacientesEgresados")]
        public async Task<IActionResult> GetPacientesEgresados()
        {
            var consulta = await (from s in _context.Seguimiento
                                  where s.Estado_Seguimiento == "FINALIZADO" && s.Estado == "Activo"
                                  select new PacienteEgresoDTO
                                  {
                                      CodigoPaciente = s.Codigo_Seguimiento,
                                      UltimoSeguimiento = s.Fecha_Final,
                                      Motivo = "Alta Médica"
                                  }).ToListAsync();
            return Ok(consulta);
        }
        //Ver medicinas pendientes por paciente
        [HttpGet("MedicinasPendientes")]
        public async Task<IActionResult> GetMedicinasPendientes()
        {
            // Obtenemos la hora actual en UTC
            DateTime utcNow = DateTime.UtcNow;

            // Definimos la zona horaria de Bolivia (UTC-4)
            TimeZoneInfo boliviaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");

            // Convertimos la hora UTC a la hora de Bolivia
            DateTime boliviaNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, boliviaTimeZone);

            // Ahora sí, tenemos el 'hoy' correcto para Tarija
            var hoy = DateOnly.FromDateTime(boliviaNow);

            var consulta = await (from ds in _context.Detalle_Seguimiento
                                  join s in _context.Seguimiento on ds.Id_Seguimiento equals s.Id_Seguimiento
                                  join e in _context.Enfermera on ds.Id_Enfermera equals e.Id_Enfermera
                                  where ds.Estado == "Activo"
                                  && s.Estado_Seguimiento != "FINALIZADO"
                                  && ds.Codigo_Receta != null
                                  && ds.Fecha_Inicio <= hoy
                                  && ds.Fecha_Final >= hoy
                                  select new MedicinaPendienteDTO
                                  {
                                      Codigo = s.Codigo_Seguimiento,
                                      Paciente = s.Codigo_Seguro,
                                      Medicamento = ds.Codigo_Receta,
                                      EnfermeraAsignada = e.Nombre + " " + e.Apellido_Paterno,
                                      FechaInicio = ds.Fecha_Inicio,
                                      FechaFinal = ds.Fecha_Final
                                  }).ToListAsync();

            return Ok(consulta);
        }
        //Ver cuántas tareas tiene cada enfermera
        [HttpGet("CargaLabora")]
        public async Task<IActionResult> CargaLaboraDTO()
        {
            var consulta = await (from ds in _context.Detalle_Seguimiento
                                  join e in _context.Enfermera on ds.Id_Enfermera equals e.Id_Enfermera
                                  where ds.Estado == "Activo"
                                  // Agrupamos por ambos campos para poder proyectarlos
                                  group ds by new { e.Codigo_Enfermera, NombreCompleto = e.Nombre + " " + e.Apellido_Paterno } into grupo
                                  select new CargaLaboraDTO
                                  {
                                      Codigo = grupo.Key.Codigo_Enfermera,
                                      Enfermera = grupo.Key.NombreCompleto,
                                      CantidadTareas = grupo.Count()
                                  }).ToListAsync();

            return Ok(consulta);
        }
        //Consultar Cobertura de Campos por Turno
        [HttpGet("CoberturaTurno")]
        public async Task<IActionResult> CoberturaTurnoDTO()
        {
            var consulta = await (from tc in _context.Turno_Campo
                                  join t in _context.Turno on tc.Id_Turno equals t.Id_Turno
                                  join c in _context.Campo on tc.Id_Campo equals c.Id_Campo
                                  where tc.Estado == "Activo"
                                  select new CoberturaTurnoDTO
                                  {
                                      Turno = t.Nombre_Turno,
                                      Cantidad = c.Cantidad
                                  }).ToListAsync();
            return Ok(consulta);
        }
        //Alertas de pacientes graves
        [HttpGet("AlertasCriticas")]
        public async Task<IActionResult> AlertaPacienteDTO()
        {
            var consulta = await (from ds in _context.Detalle_Seguimiento
                                  join s in _context.Seguimiento on ds.Id_Seguimiento equals s.Id_Seguimiento
                                  where ds.Estado == "Activo" && s.Estado_Seguimiento == "CRITICO"
                                  select new AlertaPacienteDTO
                                  {
                                      CodigoPaciente = s.Codigo_Seguro,
                                      ObservacionCritica = ds.Observacion,
                                      Fecha = ds.Fecha_Inicio
                                  }).ToListAsync();
            return Ok(consulta);
        }
        //Leer notas de enfermeras anteriores
        [HttpGet("NotasAnteriores")]
        public async Task<IActionResult> NotasAnteriores()
        {
            var consulta = await (from ds in _context.Detalle_Seguimiento
                                  join e in _context.Enfermera on ds.Id_Enfermera equals e.Id_Enfermera
                                  where ds.Estado == "Activo"
                                  select new NotasAnterioresDTO
                                  {
                                      Codigo = e.Codigo_Enfermera,
                                      NombreEnfermera = e.Nombre + " " + e.Apellido_Paterno,
                                      Nota = ds.Observacion,
                                      Fecha_Inicio = ds.Fecha_Inicio,
                                      Fecha_Final = ds.Fecha_Final
                                  }).ToListAsync();
            return Ok(consulta);
        }
        //Revisar qué enfermera está de turno
        [HttpGet("EnfermeraDeTurno/{Codigo_Enfermera}")]
        public async Task<IActionResult> EnfermeraTurnoDTO(string Codigo_Enfermera)
        {
            var consulta = await (from a in _context.Asignar
                                  join e in _context.Enfermera on a.Id_Enfermera equals e.Id_Enfermera
                                  join t in _context.Turno on a.Id_Turno equals t.Id_Turno
                                  where e.Codigo_Enfermera == Codigo_Enfermera && a.Estado == "Activo"
                                  select new EnfermeraDeTurnoDTO
                                  {
                                      Codigo = e.Codigo_Enfermera,
                                      NombreEnfermera = e.Nombre + " " + e.Apellido_Paterno,
                                      Turno = t.Nombre_Turno
                                  }).ToListAsync();
            return Ok(consulta);
        }
        //Buscar personal disponible ahora
        [HttpGet("DisponibilidadInmediata")]
        public async Task<IActionResult> GetDisponibilidadAhora()
        {
            var consulta = await (from e in _context.Enfermera
                                  where !_context.Detalle_Seguimiento.Any(ds => ds.Id_Enfermera == e.Id_Enfermera && ds.Estado == "Activo")
                                  && e.Estado == "Activo"
                                  select new
                                  {
                                      e.Codigo_Enfermera,
                                      e.Nombre,
                                      e.Apellido_Paterno
                                  }).ToListAsync();
            return Ok(consulta);
        }
        //Ver Eficiencia de Turnos por Horario
        [HttpGet("AnalisisCargaHoraria")]
        public async Task<IActionResult> GetAnalisisCarga()
        {
            var consulta = await (from t in _context.Turno
                                  join a in _context.Asignar on t.Id_Turno equals a.Id_Turno
                                  where t.Estado == "Activo" && a.Estado == "Activo"
                                  group a by new { t.Nombre_Turno, t.Hora_Inicio, t.Hora_Final } into grupo
                                  select new EficienciaTurnoDTO
                                  {
                                      Turno = grupo.Key.Nombre_Turno,
                                      RangoHorario = grupo.Key.Hora_Inicio + " a " + grupo.Key.Hora_Final,
                                      TotalAsignados = grupo.Count()
                                  }).ToListAsync();
            return Ok(consulta);
        }
        //Seguimiento de Recetas por Rango de Tiempo
        [HttpGet("ControlMedicacionActual")]
        public async Task<IActionResult> GetControlMedicacion()
        {
            var consulta = await (from ds in _context.Detalle_Seguimiento
                                  join e in _context.Enfermera on ds.Id_Enfermera equals e.Id_Enfermera
                                  join s in _context.Seguimiento on ds.Id_Seguimiento equals s.Id_Seguimiento
                                  where ds.Estado == "Activo"
                                  select new ControlRecetasDTO
                                  {
                                      Enfermera = e.Nombre + " " + e.Apellido_Paterno,
                                      CodigoReceta = ds.Codigo_Receta.ToString(),
                                      HorarioAtencion = ds.Fecha_Inicio + " - " + ds.Fecha_Final,
                                      Paciente = s.Codigo_Seguimiento
                                  }).ToListAsync();
            return Ok(consulta);
        }
    }
}
