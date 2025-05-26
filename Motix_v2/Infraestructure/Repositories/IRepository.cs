using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Motix_v2.Infraestructure.Repositories
{
    /// <summary>
    /// Contrato genérico que deben implementar todos los repositorios.
    /// No realiza la operación de guardado; esa responsabilidad recae en IUnitOfWork.
    /// </summary>
    /// <typeparam name="TEntity">Tipo de la entidad de dominio.</typeparam>
    public interface IRepository<TEntity> where TEntity : class
    {
        /* ----------- Lectura ----------- */
        Task<TEntity?> GetByIdAsync(object id, CancellationToken ct = default);
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate,
                                             CancellationToken ct = default);

        /* ----------- Escritura ----------- */
        Task AddAsync(TEntity entity, CancellationToken ct = default);
        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);
        void Update(TEntity entity);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
    }
}
