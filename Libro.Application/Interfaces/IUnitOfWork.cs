
using Microsoft.EntityFrameworkCore.Storage;

namespace Libro.Infrastructure
{
    public interface IUnitOfWork
    {
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitAsync(IDbContextTransaction contextTransaction);
        Task<int> SaveChangesAsync();
    }
}