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
        public DbSet<Rol> Roles => Set<Rol>();
        public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
        public DbSet<Part> Parts => Set<Part>();
        public DbSet<Document> Documents => Set<Document>();
        public DbSet<DocumentLine> DocumentLines => Set<DocumentLine>();



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
                  .HasColumnName("cif_nif")
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

            mb.Entity<Rol>(e =>
            {
                e.ToTable("roles");
                e.HasKey(x => x.Id);
                e.Property(x => x.Nombre)
                                .HasMaxLength(50)
                                .IsRequired();
            });
            
            mb.Entity<PaymentMethod>(e =>
                        {
                e.ToTable("formasdepago");
                e.HasKey(x => x.Id);
                e.Property(x => x.Nombre)
                                .HasMaxLength(50)
                                .IsRequired();
            });
            
            mb.Entity<Part>(e =>
                        {
                e.ToTable("piezas");
                e.HasKey(x => x.Id);
                e.Property(x => x.Referencia)
                                  .HasMaxLength(50)
                                  .IsRequired();
                e.Property(x => x.ReferenciaInterna)
                                  .HasColumnName("referenciainterna")
                                  .HasMaxLength(50)
                                  .IsRequired();
                e.Property(x => x.Nombre)
                                  .HasMaxLength(100);
                e.Property(x => x.Descripcion);
                e.Property(x => x.PrecioVenta)
                                  .HasColumnType("numeric(10,2)");
                e.Property(x => x.UnidadMedida)
                                  .HasMaxLength(20);
                e.Property(x => x.Stock);
            });
            
            mb.Entity<Document>(e =>
                        {
                e.ToTable("documentos");
                e.HasKey(x => x.Id);
                e.Property(x => x.ClienteId);
                e.Property(x => x.UsuarioId);
                e.Property(x => x.Fecha)
                                  .IsRequired();
                e.Property(x => x.TipoDocumento)
                                  .HasMaxLength(20)
                                  .IsRequired();
                e.Property(x => x.EstadoPago)
                                  .HasMaxLength(20)
                                  .IsRequired();
                e.Property(x => x.FormaPagoId);
                e.Property(x => x.Observaciones);
                e.Property(x => x.EstadoReparto)
                                  .HasMaxLength(20)
                                  .IsRequired();
                e.Property(x => x.BaseImponible);
                e.Property(x => x.Iva);
                e.Property(x => x.Total);
                e.HasMany(d => d.Lines)
                                 .WithOne(l => l.Document)
                                 .HasForeignKey(l => l.DocumentoId)
                                 .OnDelete(DeleteBehavior.Cascade);
            });
            
            mb.Entity<DocumentLine>(e =>
                        {
                e.ToTable("lineasdocumento");
                e.HasKey(x => x.Id);
                e.Property(x => x.DocumentoId)
                                  .HasColumnName("documentoid")
                                  .IsRequired();
                e.Property(x => x.PiezaId)
                                  .HasColumnName("piezaid");
                e.Property(x => x.Cantidad)
                                  .IsRequired();
                e.Property(x => x.PrecioUnitario)
                                  .HasColumnName("preciounitario")
                                  .HasColumnType("numeric(10,2)")
                                  .IsRequired();
                e.Property(x => x.TotalLinea)
                                  .HasColumnName("totallinea")
                                  .HasColumnType("numeric(10,2)")
                                  .IsRequired();
                e.HasOne(l => l.Document)
                                .WithMany(d => d.Lines)
                                .HasForeignKey(l => l.DocumentoId);
            });

        }
    }
}
