using Microsoft.EntityFrameworkCore;
using Transacciones.API.Data;
using Transacciones.API.Entities;

namespace Transacciones.API.Repositories
{
    public class TransaccionRepository : ITransaccionRepository
    {
        private readonly TransaccionesDbContext _context;

        public TransaccionRepository(TransaccionesDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Transaccion> Items, int TotalCount)> GetPagedAsync(int? productoId, string? tipoTransaccion, DateTime? fechaInicio, DateTime? fechaFin, int page, int pageSize)
        {
            var query = _context.Transacciones.AsQueryable();

            if (productoId.HasValue && productoId.Value > 0)
            {
                query = query.Where(t => t.ProductoId == productoId.Value);
            }

            if (!string.IsNullOrWhiteSpace(tipoTransaccion))
            {
                query = query.Where(t => t.TipoTransaccion.ToUpper() == tipoTransaccion.Trim().ToUpper());
            }

            if (fechaInicio.HasValue)
            {
                query = query.Where(t => t.Fecha >= fechaInicio.Value);
            }

            if (fechaFin.HasValue)
            {
                query = query.Where(t => t.Fecha <= fechaFin.Value);
            }

            int totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(t => t.Fecha)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<Transaccion>> GetByProductoIdAsync(int productoId, DateTime? fechaInicio, DateTime? fechaFin, string? tipoTransaccion)
        {
            var query = _context.Transacciones.Where(t => t.ProductoId == productoId);

            if (fechaInicio.HasValue)
            {
                query = query.Where(t => t.Fecha >= fechaInicio.Value);
            }

            if (fechaFin.HasValue)
            {
                query = query.Where(t => t.Fecha <= fechaFin.Value);
            }

            if (!string.IsNullOrWhiteSpace(tipoTransaccion))
            {
                query = query.Where(t => t.TipoTransaccion.ToUpper() == tipoTransaccion.Trim().ToUpper());
            }

            return await query.OrderByDescending(t => t.Fecha).ToListAsync();
        }

        public async Task<Transaccion?> GetByIdAsync(int id)
        {
            return await _context.Transacciones.FindAsync(id);
        }

        public async Task<Transaccion> CreateAsync(Transaccion transaccion)
        {
            _context.Transacciones.Add(transaccion);
            await _context.SaveChangesAsync();
            return transaccion;
        }

        public async Task<bool> UpdateAsync(Transaccion transaccion)
        {
            _context.Transacciones.Update(transaccion);
            return await SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var transaccion = await GetByIdAsync(id);
            if (transaccion == null) return false;
            _context.Transacciones.Remove(transaccion);
            return await SaveChangesAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
