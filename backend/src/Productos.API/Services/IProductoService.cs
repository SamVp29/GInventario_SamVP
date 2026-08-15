using Productos.API.DTOs;

namespace Productos.API.Services
{
    public interface IProductoService
    {
        Task<PagedResult<ProductoDto>> GetPagedAsync(string? busqueda, string? categoria, int page, int pageSize);
        Task<ProductoDto?> GetByIdAsync(int id);
        Task<ProductoDto> CreateAsync(CrearProductoDto dto);
        Task<ProductoDto?> UpdateAsync(int id, EditarProductoDto dto);
        Task<bool> DeleteAsync(int id);
        Task<ProductoDto?> AjustarStockAsync(int id, int cantidadDelta);
    }
}
