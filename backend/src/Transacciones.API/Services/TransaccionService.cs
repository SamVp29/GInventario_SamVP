using Transacciones.API.Clients;
using Transacciones.API.DTOs;
using Transacciones.API.Entities;
using Transacciones.API.Repositories;

namespace Transacciones.API.Services
{
    public class TransaccionService : ITransaccionService
    {
        private readonly ITransaccionRepository _repository;
        private readonly IProductoRestClient _productoRestClient;

        public TransaccionService(ITransaccionRepository repository, IProductoRestClient productoRestClient)
        {
            _repository = repository;
            _productoRestClient = productoRestClient;
        }

        public async Task<PagedResult<TransaccionDto>> GetPagedAsync(int? productoId, string? tipoTransaccion, DateTime? fechaInicio, DateTime? fechaFin, int page, int pageSize)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var (items, totalCount) = await _repository.GetPagedAsync(productoId, tipoTransaccion, fechaInicio, fechaFin, page, pageSize);

            var dtoList = new List<TransaccionDto>();
            foreach (var item in items)
            {
                dtoList.Add(await MapToDtoAsync(item));
            }

            return new PagedResult<TransaccionDto>
            {
                Items = dtoList,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<TransaccionDto>> GetHistorialPorProductoAsync(int productoId, DateTime? fechaInicio, DateTime? fechaFin, string? tipoTransaccion)
        {
            var items = await _repository.GetByProductoIdAsync(productoId, fechaInicio, fechaFin, tipoTransaccion);
            var dtoList = new List<TransaccionDto>();

            var producto = await _productoRestClient.GetProductoByIdAsync(productoId);

            foreach (var item in items)
            {
                dtoList.Add(new TransaccionDto
                {
                    Id = item.Id,
                    Fecha = item.Fecha,
                    TipoTransaccion = item.TipoTransaccion,
                    ProductoId = item.ProductoId,
                    NombreProducto = producto?.Nombre ?? $"Producto #{item.ProductoId}",
                    StockProductoActual = producto?.Stock ?? 0,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.PrecioUnitario,
                    PrecioTotal = item.PrecioTotal,
                    Detalle = item.Detalle
                });
            }

            return dtoList;
        }

        public async Task<TransaccionDto?> GetByIdAsync(int id)
        {
            var transaccion = await _repository.GetByIdAsync(id);
            if (transaccion == null) return null;
            return await MapToDtoAsync(transaccion);
        }

        public async Task<TransaccionDto> CreateAsync(CrearTransaccionDto dto)
        {
            string tipo = dto.TipoTransaccion.Trim().ToUpper();
            if (tipo != "COMPRA" && tipo != "VENTA")
            {
                throw new ArgumentException("El tipo de transacción debe ser 'COMPRA' o 'VENTA'.");
            }

            if (dto.Cantidad <= 0)
            {
                throw new ArgumentException("La cantidad debe ser mayor a cero.");
            }

            // Consultar producto síncronamente en Productos.API
            var producto = await _productoRestClient.GetProductoByIdAsync(dto.ProductoId);
            if (producto == null)
            {
                throw new KeyNotFoundException($"El producto con ID {dto.ProductoId} no existe en el inventario.");
            }

            // Regla de Negocio: Validación de stock si es una Venta
            if (tipo == "VENTA" && producto.Stock < dto.Cantidad)
            {
                throw new InvalidOperationException($"Stock insuficiente para la venta. Stock disponible: {producto.Stock}, cantidad solicitada: {dto.Cantidad}.");
            }

            var transaccion = new Transaccion
            {
                Fecha = DateTime.UtcNow,
                TipoTransaccion = tipo,
                ProductoId = dto.ProductoId,
                Cantidad = dto.Cantidad,
                PrecioUnitario = dto.PrecioUnitario,
                PrecioTotal = dto.Cantidad * dto.PrecioUnitario,
                Detalle = dto.Detalle
            };

            // Guardar transacción
            var creada = await _repository.CreateAsync(transaccion);

            // Ajustar stock síncronamente en Productos.API
            int delta = tipo == "COMPRA" ? dto.Cantidad : -dto.Cantidad;
            await _productoRestClient.AjustarStockAsync(dto.ProductoId, delta);

            return await MapToDtoAsync(creada);
        }

        public async Task<TransaccionDto?> UpdateAsync(int id, EditarTransaccionDto dto)
        {
            var transaccion = await _repository.GetByIdAsync(id);
            if (transaccion == null) return null;

            string nuevoTipo = dto.TipoTransaccion.Trim().ToUpper();
            if (nuevoTipo != "COMPRA" && nuevoTipo != "VENTA")
            {
                throw new ArgumentException("El tipo de transacción debe ser 'COMPRA' o 'VENTA'.");
            }

            if (dto.Cantidad <= 0)
            {
                throw new ArgumentException("La cantidad debe ser mayor a cero.");
            }

            // Revertir efecto de la transacción anterior
            int revertDelta = transaccion.TipoTransaccion == "COMPRA" ? -transaccion.Cantidad : transaccion.Cantidad;
            // Aplicar efecto de la nueva transacción
            int nuevoDelta = nuevoTipo == "COMPRA" ? dto.Cantidad : -dto.Cantidad;

            int netDelta = revertDelta + nuevoDelta;

            if (netDelta != 0)
            {
                await _productoRestClient.AjustarStockAsync(transaccion.ProductoId, netDelta);
            }

            transaccion.TipoTransaccion = nuevoTipo;
            transaccion.Cantidad = dto.Cantidad;
            transaccion.PrecioUnitario = dto.PrecioUnitario;
            transaccion.PrecioTotal = dto.Cantidad * dto.PrecioUnitario;
            transaccion.Detalle = dto.Detalle;

            await _repository.UpdateAsync(transaccion);
            return await MapToDtoAsync(transaccion);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var transaccion = await _repository.GetByIdAsync(id);
            if (transaccion == null) return false;

            // Reversión de stock al eliminar
            int revertDelta = transaccion.TipoTransaccion == "COMPRA" ? -transaccion.Cantidad : transaccion.Cantidad;
            await _productoRestClient.AjustarStockAsync(transaccion.ProductoId, revertDelta);

            return await _repository.DeleteAsync(id);
        }

        private async Task<TransaccionDto> MapToDtoAsync(Transaccion t)
        {
            var producto = await _productoRestClient.GetProductoByIdAsync(t.ProductoId);
            return new TransaccionDto
            {
                Id = t.Id,
                Fecha = t.Fecha,
                TipoTransaccion = t.TipoTransaccion,
                ProductoId = t.ProductoId,
                NombreProducto = producto?.Nombre ?? $"Producto #{t.ProductoId}",
                StockProductoActual = producto?.Stock ?? 0,
                Cantidad = t.Cantidad,
                PrecioUnitario = t.PrecioUnitario,
                PrecioTotal = t.PrecioTotal,
                Detalle = t.Detalle
            };
        }
    }
}
