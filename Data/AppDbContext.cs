    using Microsoft.EntityFrameworkCore;
using SaborPrestigioMVC.Models;

namespace SaborPrestigioMVC.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Rol> Roles { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Mesa> Mesas { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<CategoriaPlato> CategoriasPlatos { get; set; }
        public DbSet<Platillo> Platillos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallePedido> DetallePedidos { get; set; }
        public DbSet<Insumo> Insumos { get; set; }
        public DbSet<RecetaPlatillo> RecetasPlatillo { get; set; }
        public DbSet<ComprobantePago> ComprobantesPago { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RecetaPlatillo>()
                .HasKey(rp => new { rp.IdPlatillo, rp.IdInsumo });

            modelBuilder.Entity<RecetaPlatillo>()
                .HasOne(rp => rp.Platillo)
                .WithMany(p => p.RecetasPlatillo)
                .HasForeignKey(rp => rp.IdPlatillo);

            modelBuilder.Entity<RecetaPlatillo>()
                .HasOne(rp => rp.Insumo)
                .WithMany(i => i.RecetasPlatillo)
                .HasForeignKey(rp => rp.IdInsumo);

            modelBuilder.Entity<DetallePedido>()
                .HasKey(dp => new { dp.IdPedido, dp.IdPlatillo });
        }
    }
}
