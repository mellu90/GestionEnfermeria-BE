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

        // GET: api/Receta/Catalogo
        [HttpGet("Catalogo")]
        public async Task<IActionResult> GetCatalogoMedicamentos()
        {
            try
            {
                // 1. Creamos el cliente HTTP aquí mismo
                var client = _httpClientFactory.CreateClient();

                // 2. Definimos la URL de Render de tu compañero/equipo
                var url = "https://hospital3ernivel-farmacia.onrender.com/api/Medicamentos/catalogo";

                // 3. Hacemos la petición y mapeamos al DTO que creaste
                var catalogo = await client.GetFromJsonAsync<List<RecetaGetDTO>>(url);

                if (catalogo == null || catalogo.Count == 0)
                    return NotFound("No se encontraron medicamentos en el catálogo.");

                return Ok(catalogo);
            }
            catch (HttpRequestException ex)
            {
                // Por si el servidor de la farmacia está apagado o en reposo
                return StatusCode(500, $"Error al conectar con la farmacia: {ex.Message}");
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