using Productos.API.DTOs;
using Productos.API.Entities;
using Productos.API.Repositories;

namespace Productos.API.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _repository;

        public ProductoService(IProductoRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<ProductoDto>> GetPagedAsync(string? busqueda, string? categoria, int page, int pageSize)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var (items, totalCount) = await _repository.GetPagedAsync(busqueda, categoria, page, pageSize);

            return new PagedResult<ProductoDto>
            {
                Items = items.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<ProductoDto?> GetByIdAsync(int id)
        {
            var producto = await _repository.GetByIdAsync(id);
            return producto == null ? null : MapToDto(producto);
        }

        public async Task<ProductoDto> CreateAsync(CrearProductoDto dto)
        {
            var producto = new Producto
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Categoria = dto.Categoria,
                Imagen = dto.Imagen,
                Precio = dto.Precio,
                Stock = dto.Stock < 0 ? 0 : dto.Stock
            };

            var creado = await _repository.CreateAsync(producto);
            return MapToDto(creado);
        }

        public async Task<ProductoDto?> UpdateAsync(int id, EditarProductoDto dto)
        {
            var producto = await _repository.GetByIdAsync(id);
            if (producto == null) return null;

            producto.Nombre = dto.Nombre;
            producto.Descripcion = dto.Descripcion;
            producto.Categoria = dto.Categoria;
            producto.Imagen = dto.Imagen;
            producto.Precio = dto.Precio;
            producto.Stock = dto.Stock < 0 ? 0 : dto.Stock;

            await _repository.UpdateAsync(producto);
            return MapToDto(producto);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var producto = await _repository.GetByIdAsync(id);
            if (producto == null) return false;

            var cantTransacciones = await _repository.CountTransaccionesAsync(id);
            if (cantTransacciones > 0)
            {
                throw new InvalidOperationException($"No se puede eliminar el producto '{producto.Nombre}' porque posee {cantTransacciones} movimiento(s) registrado(s) en el historial de transacciones.");
            }

            return await _repository.DeleteAsync(id);
        }

        public async Task<ProductoDto?> AjustarStockAsync(int id, int cantidadDelta)
        {
            var producto = await _repository.GetByIdAsync(id);
            if (producto == null) return null;

            int nuevoStock = producto.Stock + cantidadDelta;
            if (nuevoStock < 0)
            {
                throw new InvalidOperationException($"Stock insuficiente para el producto {producto.Nombre}. Stock disponible: {producto.Stock}, intentó reducir en: {Math.Abs(cantidadDelta)}.");
            }

            producto.Stock = nuevoStock;
            await _repository.UpdateAsync(producto);
            return MapToDto(producto);
        }

        private static ProductoDto MapToDto(Producto p) => new ProductoDto
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            Categoria = p.Categoria,
            Imagen = p.Imagen,
            Precio = p.Precio,
            Stock = p.Stock
        };
    }
}
