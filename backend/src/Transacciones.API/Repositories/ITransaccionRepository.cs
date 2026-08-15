using Transacciones.API.Entities;

namespace Transacciones.API.Repositories
{
    public interface ITransaccionRepository
    {
        Task<(IEnumerable<Transaccion> Items, int TotalCount)> GetPagedAsync(int? productoId, string? tipoTransaccion, DateTime? fechaInicio, DateTime? fechaFin, int page, int pageSize);
        Task<IEnumerable<Transaccion>> GetByProductoIdAsync(int productoId, DateTime? fechaInicio, DateTime? fechaFin, string? tipoTransaccion);
        Task<Transaccion?> GetByIdAsync(int id);
        Task<Transaccion> CreateAsync(Transaccion transaccion);
        Task<bool> UpdateAsync(Transaccion transaccion);
        Task<bool> DeleteAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}
