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
    /// Repositorio para la entidad <see cref="Rol"/>.
    /// </summary>
    public sealed class RolRepository : IRepository<Rol>
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Rol> _dbSet;

        public RolRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<Rol>();
        }

        // ----------- Lectura -----------

        public Task<Rol?> GetByIdAsync(object key, CancellationToken ct = default) =>
            _dbSet.FindAsync(new object[] { key }, ct).AsTask();

        public async Task<IEnumerable<Rol>> GetAllAsync(CancellationToken ct = default) =>
            await _dbSet.AsNoTracking().ToListAsync(ct);

        public async Task<IEnumerable<Rol>> FindAsync(
            Expression<Func<Rol, bool>> predicate,
            CancellationToken ct = default) =>
            await _dbSet.AsNoTracking().Where(predicate).ToListAsync(ct);

        // ----------- Escritura -----------

        public async Task AddAsync(Rol entity, CancellationToken ct = default) =>
            await _dbSet.AddAsync(entity, ct);

        public async Task AddRangeAsync(IEnumerable<Rol> entities, CancellationToken ct = default) =>
            await _dbSet.AddRangeAsync(entities, ct);

        public void Update(Rol entity) =>
            _dbSet.Update(entity);

        public void Remove(Rol entity) =>
            _dbSet.Remove(entity);

        public void RemoveRange(IEnumerable<Rol> entities) =>
            _dbSet.RemoveRange(entities);
    }
}
