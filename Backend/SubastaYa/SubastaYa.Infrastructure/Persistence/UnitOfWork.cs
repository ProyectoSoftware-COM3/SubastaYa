using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Application.Exceptions;

namespace SubastaYa.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SubastaYaDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(SubastaYaDbContext context) => _context = context;

        public async Task BeginTransactionAsync(CancellationToken ct = default)
            => _transaction = await _context.Database.BeginTransactionAsync(ct);

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            try
            {
                return await _context.SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConcurrencyConflictException("la entidad modificada");
            }
        }

        public async Task CommitAsync(CancellationToken ct = default)
        {
            if (_transaction is null) return;
            await _transaction.CommitAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async Task RollbackAsync(CancellationToken ct = default)
        {
            if (_transaction is null) return;
            await _transaction.RollbackAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}