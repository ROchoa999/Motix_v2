using Motix_v2.Domain.Entities;
using Motix_v2.Infraestructure.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Motix_v2.Infraestructure.UnitOfWork
{
    /// <summary>
    /// Orquesta los repositorios y concentra el guardado atómico de cambios.
    /// </summary>
    public interface IUnitOfWork : IAsyncDisposable
    {
        /* Repositorios expuestos */
        IRepository<Customer> Customers { get; }
        IRepository<User> Users { get; }
        IRepository<Rol> Roles { get; }
        IRepository<PaymentMethod> PaymentMethods { get; }
        IRepository<Part> Parts { get; }
        IRepository<Document> Documents { get; }

        /* Persistencia */
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
