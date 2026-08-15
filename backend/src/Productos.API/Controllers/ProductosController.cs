using Microsoft.AspNetCore.Mvc;
using Productos.API.DTOs;
using Productos.API.Services;

namespace Productos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductosController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? busqueda, [FromQuery] string? categoria, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _productoService.GetPagedAsync(busqueda, categoria, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var producto = await _productoService.GetByIdAsync(id);
            if (producto == null) return NotFound(new { mensaje = $"Producto con ID {id} no encontrado." });
            return Ok(producto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearProductoDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var creado = await _productoService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EditarProductoDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var actualizado = await _productoService.UpdateAsync(id, dto);
            if (actualizado == null) return NotFound(new { mensaje = $"Producto con ID {id} no encontrado." });
            return Ok(actualizado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var borrado = await _productoService.DeleteAsync(id);
                if (!borrado) return NotFound(new { mensaje = $"Producto con ID {id} no encontrado." });
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> AjustarStock(int id, [FromBody] AjustarStockDto dto)
        {
            try
            {
                var actualizado = await _productoService.AjustarStockAsync(id, dto.CantidadDelta);
                if (actualizado == null) return NotFound(new { mensaje = $"Producto con ID {id} no encontrado." });
                return Ok(actualizado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
