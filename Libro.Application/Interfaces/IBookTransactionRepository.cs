using Libro.Domain.Entities;

namespace Libro.Application.Interfaces
{
    public interface IBookTransactionRepository
    {
        Task ReserveBookAsync(BookTransaction bookTransaction);
        Task CheckOutAsync(Guid userId, Guid bookId, DateTime dueDate);
        Task ReturnBookAsync(Guid userId, Guid bookId);
        
    }
}