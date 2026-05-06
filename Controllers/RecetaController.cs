using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GestionEnfermeria.DTO;
using System.Net.Http.Json; // <-- Súper importante para deserializar el JSON

namespace GestionEnfermeria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecetaController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Inyectamos la fábrica directamente en el controller
        public RecetaController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: api/Receta/Pendientes
        [HttpGet("Pendientes")]
        public async Task<IActionResult> GetRecetasPendientes()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();

                // Nueva URL de la farmacia
                var url = "https://hospital3ernivel-farmacia.onrender.com/api/Recetas/receta/pendientes";

                // Obtenemos la lista completa con sus detalles y posología
                var recetas = await client.GetFromJsonAsync<List<RecetaGetDTO>>(url);

                if (recetas == null || recetas.Count == 0)
                    return NotFound("No se encontraron recetas pendientes en el sistema de farmacia.");

                return Ok(recetas);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error al conectar con farmacia (Render): {ex.Message}");
            }
        }

        // POST: api/Receta/Asignar
        [HttpPost("Asignar")]
        public async Task<IActionResult> PostReceta([FromBody] RecetaPostDTO nuevaReceta)
        {
            // 1. Validamos que los datos no lleguen vacíos
            if (string.IsNullOrWhiteSpace(nuevaReceta.recetaCodigo) || string.IsNullOrWhiteSpace(nuevaReceta.enfermeraCodigo))
            {
                return BadRequest("El código de la receta y de la enfermera son obligatorios.");
            }

            try
            {
                // 2. Creamos el cliente HTTP
                var client = _httpClientFactory.CreateClient();

                // 3. La URL de la farmacia para dispensaciones
                var urlFarmacia = "https://hospital3ernivel-farmacia.onrender.com/api/Dispensaciones/enfermeria";

                // 4. Enviamos el objeto nuevaReceta (JSON) a la farmacia
                // Usamos PostAsJsonAsync que facilita mucho el envío de datos
                var response = await client.PostAsJsonAsync(urlFarmacia, nuevaReceta);

                // 5. Verificamos si la farmacia aceptó la petición
                if (response.IsSuccessStatusCode)
                {
                    // Opcional: Aquí podrías guardar también en TU base de datos local
                    // para tener un registro de qué enfermera pidió qué cosa.

                    return Ok(new
                    {
                        mensaje = "Solicitud enviada a farmacia exitosamente.",
                        statusExterno = response.StatusCode,
                        datos = nuevaReceta
                    });
                }
                else
                {
                    // Si la farmacia responde con un error (ej. 400 o 500)
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"La farmacia rechazó la solicitud: {errorContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                // Manejo por si el servidor de farmacia no responde (muy común en Render por el modo reposo)
                return StatusCode(500, $"Error de conexión con el servicio de farmacia: {ex.Message}");
            }
        }
    }
}