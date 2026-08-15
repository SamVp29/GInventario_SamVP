namespace Productos.API.DTOs
{
    public class ProductoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Imagen { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }

    public class CrearProductoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Imagen { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }

    public class EditarProductoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Imagen { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }

    public class AjustarStockDto
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
