using Libro.Domain.Entities;

namespace Libro.Application.Interfaces
{
    public interface IBookTransactionRepository
    {
        Task<BookTransaction> GetUserBookTransactionAsync(Guid UserId, Guid TransactionId);
        Task AddBookTransactionWithReservedStatus(BookTransaction bookTransaction);
        void ChangeBookTransactionStatusToBorrowed(BookTransaction bookTransaction, DateTime DueDate);
        void ChangeBookTransactionStatusToNone(BookTransaction bookTransaction);
        Task<(List<BookTransaction>, int)> TrackDueDateAsync(int PageNumber, int Count);
        Task<BookTransaction> GetBookTransactionWhereStatusNotNone(Guid TransactionId);
        void DeleteBookTransaction(BookTransaction Transaction, Book book);
        Task<bool> BookIsReturnedAsync(Guid UserId, Guid BookId);


    }
}