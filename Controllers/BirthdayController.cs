using birthday_notification_api.Models;
using birthday_notification_api.Models.Models;
using birthday_notification_api.Servicios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace birthday_notification_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BirthdayController : ControllerBase
    {
        private readonly BirthdayService _birthdayService;

        public BirthdayController(BirthdayService birthdayService)
        {
            _birthdayService = birthdayService;
        }

        [HttpGet]
        public async Task<ActionResult<List<BirthdayDTO>>> ObtenerTodos()
        {
            var todos = await _birthdayService.ObtenerTodos();
            return Ok(todos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BirthdayDTO>> ObtenerPorId(int id)
        {
            var persona = await _birthdayService.ObtenerPorId(id);
            if (persona == null)
            {
               return NotFound(new { mensaje = "Persona no encontrada" });
            }

            return Ok(persona);
        }

        [HttpPost]
        public async Task<IActionResult> CrearPersona([FromBody] Birthday persona)
        {
            if (string.IsNullOrEmpty(persona.nombre))
            {
                return BadRequest(new { mensaje = "El nombre es obligatorio" });
            }

            if (string.IsNullOrEmpty(persona.telefono))
            {
                return BadRequest(new { mensaje = "El telefono es obligatorio" });
            }

            persona.url_imag = _birthdayService.SubirImg(persona.url_imag);

            var success = await _birthdayService.CrearPersona(persona);

            if (success)
            {
                return CreatedAtAction(nameof(ObtenerPorId), new { id = persona.id }, persona);
            }

            return StatusCode(500, new { mensaje = "Error al crear el registro" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarPersona(int id, [FromBody] Birthday persona)
        {
            var existepersona = await _birthdayService.ObtenerPorId(persona.id);
            if (existepersona == null)
            {
                return NotFound(new { mensaje = "Persona no encontrada" });
            }

            if (id != persona.id)
            {
                return BadRequest(new { mensaje = "El ID en la URL no coincide con el ID del servicio." });
            }

            if (string.IsNullOrEmpty(persona.nombre))
            {
                return BadRequest(new { mensaje = "El nombre es obligatorio" });
            }

            if (string.IsNullOrEmpty(persona.telefono))
            {
                return BadRequest(new { mensaje = "El telefono es obligatorio" });
            }

            var valor = UrlValida(persona.url_imag);
            if (!valor)
            {
                persona.url_imag = _birthdayService.SubirImg(persona.url_imag);
            }

            var success = await _birthdayService.ActualizarPersona(persona);
            if (success)
            {
                return Ok(new { mensaje = "Servicio actualizado correctamente." });
            }

            return StatusCode(500, new { mensaje = "Error al actualizar a la persona." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarServicio(int id)
        {
            var success = await _birthdayService.EliminarPersona(id);

            if (!success)
            {
                return NotFound(new { mensaje = "Persona no encontrada o ya eliminado." });
            }

            return Ok(new { mensaje = "Persona eliminada correctamente." });
        }

        public static bool UrlValida(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
