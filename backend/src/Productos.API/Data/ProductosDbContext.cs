using Microsoft.EntityFrameworkCore;
using Productos.API.Entities;

namespace Productos.API.Data
{
    public class ProductosDbContext : DbContext
    {
        public ProductosDbContext(DbContextOptions<ProductosDbContext> options) : base(options) { }

        public DbSet<Producto> Productos => Set<Producto>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("productos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Nombre).HasColumnName("nombre").IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasColumnName("descripcion").IsRequired().HasMaxLength(255);
                entity.Property(e => e.Categoria).HasColumnName("categoria").IsRequired().HasMaxLength(50);
                entity.Property(e => e.Imagen).HasColumnName("imagen").IsRequired().HasMaxLength(500);
                entity.Property(e => e.Precio).HasColumnName("precio").HasPrecision(18, 2);
                entity.Property(e => e.Stock).HasColumnName("stock");
            });
        }
    }
}
