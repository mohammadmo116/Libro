using Libro.Domain.Entities;

namespace Libro.Application.Interfaces
{
    public interface IBookTransactionRepository
    {
        Task ReserveBook(BookTransaction bookTransaction);
        Task CheckOut(Guid userId, Guid bookId, DateTime dueDate);
    }
}