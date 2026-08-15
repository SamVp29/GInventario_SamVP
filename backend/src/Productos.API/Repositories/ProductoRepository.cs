using Microsoft.EntityFrameworkCore;
using Productos.API.Data;
using Productos.API.Entities;

namespace Productos.API.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly ProductosDbContext _context;

        public ProductoRepository(ProductosDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Producto> Items, int TotalCount)> GetPagedAsync(string? busqueda, string? categoria, int page, int pageSize)
        {
            var query = _context.Productos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var term = busqueda.Trim().ToLower();
                query = query.Where(p => p.Nombre.ToLower().Contains(term) || p.Descripcion.ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(categoria))
            {
                query = query.Where(p => p.Categoria.ToLower() == categoria.Trim().ToLower());
            }

            int totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Producto?> GetByIdAsync(int id)
        {
            return await _context.Productos.FindAsync(id);
        }

        public async Task<Producto> CreateAsync(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return producto;
        }

        public async Task<bool> UpdateAsync(Producto producto)
        {
            _context.Productos.Update(producto);
            return await SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var producto = await GetByIdAsync(id);
            if (producto == null) return false;
            _context.Productos.Remove(producto);
            return await SaveChangesAsync();
        }

        public async Task<int> CountTransaccionesAsync(int productoId)
        {
            return await _context.Database
                .SqlQueryRaw<int>("SELECT COUNT(1) AS \"Value\" FROM transacciones WHERE productoid = {0}", productoId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
