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
    /// Repositorio para la entidad <c>Document</c>, que maneja
    /// tanto los documentos como sus líneas.
    /// </summary>
    public sealed class DocumentRepository : IRepository<Document>
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Document> _dbSet;
        private readonly DbSet<DocumentLine> _linesSet;

        public DocumentRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<Document>();
            _linesSet = context.Set<DocumentLine>();
        }

        // ----------- Lectura de documentos -----------

        public Task<Document?> GetByIdAsync(object key, CancellationToken ct = default) =>
            _dbSet.FindAsync(new object[] { key }, ct).AsTask();

        public async Task<IEnumerable<Document>> GetAllAsync(CancellationToken ct = default) =>
            await _dbSet.AsNoTracking().ToListAsync(ct);

        public async Task<IEnumerable<Document>> FindAsync(
            Expression<Func<Document, bool>> predicate,
            CancellationToken ct = default) =>
            await _dbSet.AsNoTracking().Where(predicate).ToListAsync(ct);

        // ----------- Escritura de documentos -----------

        public async Task AddAsync(Document entity, CancellationToken ct = default) =>
            await _dbSet.AddAsync(entity, ct);

        public async Task AddRangeAsync(IEnumerable<Document> entities, CancellationToken ct = default) =>
            await _dbSet.AddRangeAsync(entities, ct);

        public void Update(Document entity) =>
            _dbSet.Update(entity);

        public void Remove(Document entity) =>
            _dbSet.Remove(entity);

        public void RemoveRange(IEnumerable<Document> entities) =>
            _dbSet.RemoveRange(entities);

        // ----------- Operaciones sobre líneas de documento -----------

        public async Task<IEnumerable<DocumentLine>> GetLinesByDocumentIdAsync(
            string documentId,
            CancellationToken ct = default) =>
            await _linesSet
                .AsNoTracking()
                .Where(l => l.DocumentoId == documentId)
                .ToListAsync(ct);

        public async Task AddLineAsync(DocumentLine line, CancellationToken ct = default) =>
            await _linesSet.AddAsync(line, ct);

        public void UpdateLine(DocumentLine line) =>
            _linesSet.Update(line);

        public void RemoveLine(DocumentLine line) =>
            _linesSet.Remove(line);

        public void RemoveLinesByDocumentId(string documentId) =>
            _linesSet.RemoveRange(
                _linesSet.Where(l => l.DocumentoId == documentId));
    }
}
