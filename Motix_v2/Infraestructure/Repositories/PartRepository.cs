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
    public sealed class PartRepository : IRepository<Part>
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Part> _dbSet;

        public PartRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<Part>();
        }

        // ----------- Lectura -----------

        public Task<Part?> GetByIdAsync(object key, CancellationToken ct = default) =>
            _dbSet.FindAsync(new object[] { key }, ct).AsTask();

        public async Task<IEnumerable<Part>> GetAllAsync(CancellationToken ct = default) =>
            await _dbSet.AsNoTracking().ToListAsync(ct);

        public async Task<IEnumerable<Part>> FindAsync(
            Expression<Func<Part, bool>> predicate,
            CancellationToken ct = default) =>
            await _dbSet.AsNoTracking().Where(predicate).ToListAsync(ct);

        // ----------- Escritura -----------

        public async Task AddAsync(Part entity, CancellationToken ct = default) =>
            await _dbSet.AddAsync(entity, ct);

        public async Task AddRangeAsync(IEnumerable<Part> entities, CancellationToken ct = default) =>
            await _dbSet.AddRangeAsync(entities, ct);

        public void Update(Part entity) =>
            _dbSet.Update(entity);

        public void Remove(Part entity) =>
            _dbSet.Remove(entity);

        public void RemoveRange(IEnumerable<Part> entities) =>
            _dbSet.RemoveRange(entities);
    }
}
