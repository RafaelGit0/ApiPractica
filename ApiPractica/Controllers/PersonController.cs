using ApiPractica.Interfaces;
using ApiPractica.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiPractica.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController(IPersonService personService) : ControllerBase
    {
        private readonly IPersonService _personService = personService;

        // 📄 LISTAR
        [HttpGet("Listar")]
        public async Task<IActionResult> ListarPersonas()
        {
            var personas = await _personService.GetAll();
            return Ok(personas);
        }

        // 🔍 GET POR ID (EDIT / DELETE CONFIRM)
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var persona = await _personService.GetById(id);

            if (persona == null)
                return NotFound();

            return Ok(persona);
        }

        // ➕ AGREGAR
        [HttpPost("Agregar")]
        public async Task<IActionResult> Agregar([FromBody] Person person)
        {
            var filas = await _personService.AddUser(person);
            return Ok(filas);
        }

        // ✏️ MODIFICAR
        [HttpPut("Modificar/{id:int}")]
        public async Task<IActionResult> Modificar(int id, [FromBody] Person person)
        {
            if (id != person.Id)
                return BadRequest();

            var filas = await _personService.UpdateUser(person);

            if (filas == 0)
                return NotFound();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        // 🗑️ ELIMINAR (CONFIRMADO)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var filas = await _personService.DeleteUser(id);

            if (filas == 0)
                return NotFound();

            return NoContent();
        }
    }
}
