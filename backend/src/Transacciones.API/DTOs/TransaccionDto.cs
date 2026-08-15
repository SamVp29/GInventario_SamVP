namespace Transacciones.API.DTOs
{
    public class TransaccionDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string TipoTransaccion { get; set; } = string.Empty; // "COMPRA" o "VENTA"
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int StockProductoActual { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal PrecioTotal { get; set; }
        public string Detalle { get; set; } = string.Empty;
    }

    public class CrearTransaccionDto
    {
        public string TipoTransaccion { get; set; } = string.Empty; // "COMPRA" o "VENTA"
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public string Detalle { get; set; } = string.Empty;
    }

    public class EditarTransaccionDto
    {
        public string TipoTransaccion { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public string Detalle { get; set; } = string.Empty;
    }

    public class ProductoResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }

    public class AjustarStockRequestDto
    {
        public int CantidadDelta { get; set; }
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
