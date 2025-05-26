using Motix.Domain.Entities;
using Motix.Infraestructure.Data;
using Motix.Infraestructure.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Motix.Infraestructure.UnitOfWork
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Customers = new CustomerRepository(_context);
            Users = new UserRepository(_context);
            Roles = new RolRepository(_context);
            PaymentMethods = new PaymentMethodRepository(_context);
            Parts = new PartRepository(_context);
            Documents = new DocumentRepository(_context);
        }

        /* Repositorios expuestos */
        public IRepository<Customer> Customers { get; }
        public IRepository<User> Users { get; }
        public IRepository<Rol> Roles { get; }
        public IRepository<PaymentMethod> PaymentMethods { get; }
        public IRepository<Part> Parts { get; }
        public IRepository<Document> Documents { get; }

        /* Persistencia de cambios */
        public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
            _context.SaveChangesAsync(ct);

        /* Liberación de recursos */
        public ValueTask DisposeAsync() => _context.DisposeAsync();
    }
}
