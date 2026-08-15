using Transacciones.API.DTOs;

namespace Transacciones.API.Clients
{
    public interface IProductoRestClient
    {
        Task<ProductoResponseDto?> GetProductoByIdAsync(int productoId);
        Task<bool> AjustarStockAsync(int productoId, int cantidadDelta);
    }
}
