using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Motix_v2.Domain.Entities;    // la entidad User
using Motix_v2.Infraestructure.Data; // AppDbContext

namespace Motix_v2.Infraestructure.Repositories
{
    /// <summary>
    /// Repositorio para la entidad <see cref="User"/>.
    /// </summary>
    public sealed class UserRepository : IRepository<User>
    {
        private readonly AppDbContext _context;
        private readonly DbSet<User> _dbSet;

        public UserRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<User>();
        }

        public Task<User?> GetByIdAsync(object key, CancellationToken ct = default) =>
            _dbSet.FindAsync(new object[] { key }, ct).AsTask();

        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default) =>
            await _dbSet.AsNoTracking().ToListAsync(ct);

        public async Task<IEnumerable<User>> FindAsync(
            Expression<Func<User, bool>> predicate,
            CancellationToken ct = default) =>
            await _dbSet.AsNoTracking().Where(predicate).ToListAsync(ct);

        public async Task AddAsync(User entity, CancellationToken ct = default) =>
            await _dbSet.AddAsync(entity, ct);

        public async Task AddRangeAsync(IEnumerable<User> entities, CancellationToken ct = default) =>
            await _dbSet.AddRangeAsync(entities, ct);

        public void Update(User entity) =>
            _dbSet.Update(entity);

        public void Remove(User entity) =>
            _dbSet.Remove(entity);

        public void RemoveRange(IEnumerable<User> entities) =>
            _dbSet.RemoveRange(entities);
    }
}
