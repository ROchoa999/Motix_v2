using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Motix.Domain.Entities;
using Motix.Infraestructure.Data;

namespace Motix.Infraestructure.Repositories
{
    /// <summary>
    /// Repositorio para la entidad <see cref="PaymentMethod"/>.
    /// </summary>
    public sealed class PaymentMethodRepository : IRepository<PaymentMethod>
    {
        private readonly AppDbContext _context;
        private readonly DbSet<PaymentMethod> _dbSet;

        public PaymentMethodRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<PaymentMethod>();
        }

        // ----------- Lectura -----------

        public Task<PaymentMethod?> GetByIdAsync(object key, CancellationToken ct = default) =>
            _dbSet.FindAsync(new object[] { key }, ct).AsTask();

        public async Task<IEnumerable<PaymentMethod>> GetAllAsync(CancellationToken ct = default) =>
            await _dbSet.AsNoTracking().ToListAsync(ct);

        public async Task<IEnumerable<PaymentMethod>> FindAsync(
            Expression<Func<PaymentMethod, bool>> predicate,
            CancellationToken ct = default) =>
            await _dbSet.AsNoTracking().Where(predicate).ToListAsync(ct);

        // ----------- Escritura -----------

        public async Task AddAsync(PaymentMethod entity, CancellationToken ct = default) =>
            await _dbSet.AddAsync(entity, ct);

        public async Task AddRangeAsync(IEnumerable<PaymentMethod> entities, CancellationToken ct = default) =>
            await _dbSet.AddRangeAsync(entities, ct);

        public void Update(PaymentMethod entity) =>
            _dbSet.Update(entity);

        public void Remove(PaymentMethod entity) =>
            _dbSet.Remove(entity);

        public void RemoveRange(IEnumerable<PaymentMethod> entities) =>
            _dbSet.RemoveRange(entities);
    }
}
