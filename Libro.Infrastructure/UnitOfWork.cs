using Microsoft.EntityFrameworkCore.Storage;

namespace Libro.Infrastructure
{
    internal sealed class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync()
        {

            return await _context.SaveChangesAsync();
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();

        }
        public async Task CommitAsync(IDbContextTransaction contextTransaction)
        {

            await contextTransaction.CommitAsync();

        }
    }
}
