using Microsoft.EntityFrameworkCore;
using Transacciones.API.Entities;

namespace Transacciones.API.Data
{
    public class TransaccionesDbContext : DbContext
    {
        public TransaccionesDbContext(DbContextOptions<TransaccionesDbContext> options) : base(options) { }

        public DbSet<Transaccion> Transacciones => Set<Transaccion>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transaccion>(entity =>
            {
                entity.ToTable("transacciones");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Fecha).HasColumnName("fecha");
                entity.Property(e => e.TipoTransaccion).HasColumnName("tipotransaccion").IsRequired().HasMaxLength(20);
                entity.Property(e => e.ProductoId).HasColumnName("productoid");
                entity.Property(e => e.Cantidad).HasColumnName("cantidad");
                entity.Property(e => e.PrecioUnitario).HasColumnName("preciounitario").HasPrecision(18, 2);
                entity.Property(e => e.PrecioTotal).HasColumnName("preciototal").HasPrecision(18, 2);
                entity.Property(e => e.Detalle).HasColumnName("detalle").IsRequired().HasMaxLength(255);
            });
        }
    }
}
