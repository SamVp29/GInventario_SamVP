namespace Transacciones.API.Entities
{
    public class Transaccion
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public string TipoTransaccion { get; set; } = string.Empty; // "COMPRA" o "VENTA"
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal PrecioTotal { get; set; }
        public string Detalle { get; set; } = string.Empty;
    }
}
