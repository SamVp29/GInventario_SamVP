using Microsoft.AspNetCore.Mvc;
using Transacciones.API.DTOs;
using Transacciones.API.Services;

namespace Transacciones.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransaccionesController : ControllerBase
    {
        private readonly ITransaccionService _transaccionService;

        public TransaccionesController(ITransaccionService transaccionService)
        {
            _transaccionService = transaccionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? productoId,
            [FromQuery] string? tipoTransaccion,
            [FromQuery] DateTime? fechaInicio,
            [FromQuery] DateTime? fechaFin,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _transaccionService.GetPagedAsync(productoId, tipoTransaccion, fechaInicio, fechaFin, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var transaccion = await _transaccionService.GetByIdAsync(id);
            if (transaccion == null) return NotFound(new { mensaje = $"Transacción con ID {id} no encontrada." });
            return Ok(transaccion);
        }

        [HttpGet("historial/producto/{productoId}")]
        public async Task<IActionResult> GetHistorialPorProducto(
            int productoId,
            [FromQuery] DateTime? fechaInicio,
            [FromQuery] DateTime? fechaFin,
            [FromQuery] string? tipoTransaccion)
        {
            var historial = await _transaccionService.GetHistorialPorProductoAsync(productoId, fechaInicio, fechaFin, tipoTransaccion);
            return Ok(historial);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearTransaccionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var creada = await _transaccionService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = creada.Id }, creada);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EditarTransaccionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var actualizada = await _transaccionService.UpdateAsync(id, dto);
                if (actualizada == null) return NotFound(new { mensaje = $"Transacción con ID {id} no encontrada." });
                return Ok(actualizada);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var borrada = await _transaccionService.DeleteAsync(id);
                if (!borrada) return NotFound(new { mensaje = $"Transacción con ID {id} no encontrada." });
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
