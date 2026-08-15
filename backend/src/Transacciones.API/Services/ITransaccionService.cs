using Transacciones.API.DTOs;

namespace Transacciones.API.Services
{
    public interface ITransaccionService
    {
        Task<PagedResult<TransaccionDto>> GetPagedAsync(int? productoId, string? tipoTransaccion, DateTime? fechaInicio, DateTime? fechaFin, int page, int pageSize);
        Task<IEnumerable<TransaccionDto>> GetHistorialPorProductoAsync(int productoId, DateTime? fechaInicio, DateTime? fechaFin, string? tipoTransaccion);
        Task<TransaccionDto?> GetByIdAsync(int id);
        Task<TransaccionDto> CreateAsync(CrearTransaccionDto dto);
        Task<TransaccionDto?> UpdateAsync(int id, EditarTransaccionDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
