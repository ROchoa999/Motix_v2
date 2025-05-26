using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Motix_v2.Domain.Entities;
using Motix_v2.Infraestructure.Data;

namespace Motix_v2.Infraestructure.Repositories
{
    /// <summary>
    /// Implementación concreta del repositorio para la tabla <c>Clientes</c>.
    /// </summary>
    public sealed class CustomerRepository : IRepository<Customer>
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Customer> _dbSet;

        public CustomerRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<Customer>();          // apunta a la tabla Clientes
        }

        /* ---------- Lectura ---------- */

        public Task<Customer?> GetByIdAsync(object key, CancellationToken ct = default) =>
            _dbSet.FindAsync(new object[] { key }, ct).AsTask();

        public async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken ct = default) =>
            await _dbSet.AsNoTracking().ToListAsync(ct);

        public async Task<IEnumerable<Customer>> FindAsync(
            Expression<Func<Customer, bool>> predicate,
            CancellationToken ct = default) =>
            await _dbSet.AsNoTracking().Where(predicate).ToListAsync(ct);

        /* ---------- Escritura ---------- */

        public async Task AddAsync(Customer entity, CancellationToken ct = default) =>
            await _dbSet.AddAsync(entity, ct);

        public async Task AddRangeAsync(IEnumerable<Customer> entities, CancellationToken ct = default) =>
            await _dbSet.AddRangeAsync(entities, ct);

        public void Update(Customer entity) =>
            _dbSet.Update(entity);                       // el cambio se persiste al SaveChangesAsync

        public void Remove(Customer entity) =>
            _dbSet.Remove(entity);

        public void RemoveRange(IEnumerable<Customer> entities) =>
            _dbSet.RemoveRange(entities);
    }
}
