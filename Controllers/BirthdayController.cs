using birthday_notification_api.Models;
using birthday_notification_api.Models.Models;
using birthday_notification_api.Servicios;
using CloudinaryDotNet.Actions;
using DotNetEnv;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace birthday_notification_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BirthdayController : ControllerBase
    {
        private readonly BirthdayService _birthdayService;
        private readonly IWebHostEnvironment _webHost;

        public BirthdayController(BirthdayService birthdayService, IWebHostEnvironment webHost)
        {
            _birthdayService = birthdayService;
            _webHost = webHost;
        }

        [HttpGet]
        [Route("ObtenerTodos")]
        public async Task<ActionResult<List<BirthdayDTO>>> ObtenerTodos()
        {
            var todos = await _birthdayService.ObtenerTodos();
            return Ok(todos);
        }

        [HttpGet]
        [Route("ObtenerPorId/{id}")]
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
        [Route("CrearPersona")]
        public async Task<IActionResult> CrearPersona(
            [FromForm] Birthday persona, // Cambia [FromBody] por [FromForm]
            IFormFile imagen) // Recibe la imagen como archivo separado
        {
            if (string.IsNullOrEmpty(persona.nombre))
                return BadRequest(new { mensaje = "El nombre es obligatorio" });

            if (string.IsNullOrEmpty(persona.telefono))
                return BadRequest(new { mensaje = "El telefono es obligatorio" });

            if (imagen == null || imagen.Length == 0)
                return BadRequest(new { mensaje = "La imagen es obligatoria" });

            try
            {
                string fileName = imagen.FileName;
                string filePath = Path.Combine("uploads", fileName);
                var stream = new FileStream(filePath, FileMode.Create);
                imagen.CopyTo(stream);
                string img_route = stream.Name;
                stream.Close();

                persona.url_img = _birthdayService.SubirImg(img_route);

                System.IO.File.Delete(filePath);

                var success = await _birthdayService.CrearPersona(persona);

                if (success)
                {
                    return CreatedAtAction(nameof(ObtenerPorId), new { id = persona.id }, persona);
                }
                    
                return StatusCode(500, new { mensaje = "Error al crear el registro" });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //public async Task<IActionResult> CrearPersona([FromBody] Birthday persona)
        //{
        //    if (string.IsNullOrEmpty(persona.nombre))
        //    {
        //        return BadRequest(new { mensaje = "El nombre es obligatorio" });
        //    }

        //    if (string.IsNullOrEmpty(persona.telefono))
        //    {
        //        return BadRequest(new { mensaje = "El telefono es obligatorio" });
        //    }

        //    persona.url_img = _birthdayService.SubirImg(persona.url_img);

        //    var success = await _birthdayService.CrearPersona(persona);

        //    if (success)
        //    {
        //        return CreatedAtAction(nameof(ObtenerPorId), new { id = persona.id }, persona);
        //    }

        //    return StatusCode(500, new { mensaje = "Error al crear el registro" });
        //}

        [HttpPut]
        [Route("ActualizarPersona/{id}")]
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

            var valor = UrlValida(persona.url_img);
            if (!valor)
            {
                persona.url_img = _birthdayService.SubirImg(persona.url_img);
            }

            var success = await _birthdayService.ActualizarPersona(persona);
            if (success)
            {
                return Ok(new { mensaje = "Servicio actualizado correctamente." });
            }

            return StatusCode(500, new { mensaje = "Error al actualizar a la persona." });
        }

        [HttpDelete]
        [Route("EliminarPersona/{id}")]
        public async Task<IActionResult> EliminarPersona(int id)
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
