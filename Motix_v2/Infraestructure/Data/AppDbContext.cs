using Microsoft.EntityFrameworkCore;
using Motix_v2.Domain.Entities;         // ← contendrá la clase Customer

namespace Motix_v2.Infraestructure.Data
{
    /// <summary>
    /// DbContext principal: gestiona la conexión a PostgreSQL (Aiven)
    /// y el mapeo de entidades.
    /// </summary>
    public sealed class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // ---------- DbSets ----------
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<User> Users => Set<User>();


        // ---------- Configuración ----------
        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

            mb.Entity<Customer>(e =>
            {
                e.ToTable("clientes");
                e.HasKey(x => x.Id);

                e.Property(x => x.Nombre)
                  .HasMaxLength(100)
                  .IsRequired();

                e.Property(x => x.Direccion);

                e.Property(x => x.Telefono)
                  .HasMaxLength(20);

                e.Property(x => x.Email)
                  .HasMaxLength(100);

                e.Property(x => x.CifNif)
                  .HasColumnName("CIF_NIF")
                  .HasMaxLength(20);
            });

            mb.Entity<User>(e =>
            {
                e.ToTable("usuarios");
                e.HasKey(x => x.Id);

                e.Property(x => x.Nombre)
                  .HasMaxLength(100)
                  .IsRequired();

                e.Property(x => x.Usuario)
                  .HasMaxLength(50)
                  .IsRequired();

                e.Property(x => x.ContrasenaHash)
                  .IsRequired();

                e.Property(x => x.RolId);
            });

        }
    }
}
