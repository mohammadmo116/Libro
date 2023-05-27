using Libro.Domain.Entities;

namespace Libro.Application.Interfaces
{
    public interface IBookTransactionRepository
    {
        Task ReserveBookAsync(BookTransaction bookTransaction);
        Task CheckOutAsync(Guid TransactionId);
        Task ReturnBookAsync(Guid TransactionId);
        Task<List<BookTransaction>> TrackDueDate()


    }
}