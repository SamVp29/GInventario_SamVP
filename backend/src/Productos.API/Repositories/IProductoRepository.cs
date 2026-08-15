using Productos.API.Entities;

namespace Productos.API.Repositories
{
    public interface IProductoRepository
    {
        Task<(IEnumerable<Producto> Items, int TotalCount)> GetPagedAsync(string? busqueda, string? categoria, int page, int pageSize);
        Task<Producto?> GetByIdAsync(int id);
        Task<Producto> CreateAsync(Producto producto);
        Task<bool> UpdateAsync(Producto producto);
        Task<bool> DeleteAsync(int id);
        Task<int> CountTransaccionesAsync(int productoId);
        Task<bool> SaveChangesAsync();
    }
}
