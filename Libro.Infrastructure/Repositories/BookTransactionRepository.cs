using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Libro.Infrastructure.Repositories
{
    public class BookTransactionRepository : IBookTransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public BookTransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<BookTransaction> GetUserBookTransactionAsync(Guid UserId, Guid TransactionId)
        {
            var bookTransaction = await _context.BookTransactions
                .Include(a => a.Book)
                .Where(a => a.UserId == UserId)
                .FirstOrDefaultAsync(a => a.Id == TransactionId);

            return bookTransaction;
        }
        public async Task<bool> BookIsReturnedAsync(Guid UserId, Guid BookId)
        {
            return await _context.BookTransactions
                    .Where(a => a.UserId == UserId)
                    .Where(a => a.BookId == BookId)
                    .Where(a => a.Status == BookStatus.Returned)
                    .AnyAsync();
        }
        public async Task AddBookTransactionWithReservedStatus(BookTransaction bookTransaction)
        {

            await _context.BookTransactions.AddAsync(bookTransaction);
        }


        public void ChangeBookTransactionStatusToBorrowed(BookTransaction bookTransaction, DateTime DueDate)
        {
            bookTransaction.Status = BookStatus.Borrowed;
            bookTransaction.BorrowedDate = DateTime.UtcNow;
            bookTransaction.DueDate = DueDate;
            _context.BookTransactions.Update(bookTransaction);

        }
        public void ChangeBookTransactionStatusToNone(BookTransaction bookTransaction)
        {
            bookTransaction.Status = BookStatus.Returned;
            _context.BookTransactions.Update(bookTransaction);
        }
        public void DeleteBookTransaction(BookTransaction Transaction, Book book)
        {
            _context.BookTransactions.Remove(Transaction);
        }
        public async Task<BookTransaction> GetBookTransactionWhereStatusNotNone(Guid TransactionId)
        {
            var bookTransaction = await _context.BookTransactions
                             .Where(a => a.Status != BookStatus.Returned)
                             .FirstOrDefaultAsync(a => a.Id == TransactionId);
            return bookTransaction;
        }

        public async Task<(List<BookTransaction>, int)> TrackDueDateAsync(int PageNumber, int Count)
        {
            var bookTransactionsCount = await _context.BookTransactions
                .Include(a => a.User)
                .Include(a => a.Book)
                .Where(a => a.Status == BookStatus.Borrowed)
                .CountAsync();

            var bookTransactions = await _context.BookTransactions
                .Include(a => a.User)
                .Include(a => a.Book)
                .Where(a => a.Status == BookStatus.Borrowed)
                .OrderByDescending(a => a.DueDate)
                .Skip(PageNumber * Count)
                .Take(Count)
                .ToListAsync();

            var NumberOfPages = 1;
            if (bookTransactionsCount > 0)
                NumberOfPages = (int)Math.Ceiling((double)bookTransactionsCount / Count);

            return (bookTransactions, NumberOfPages);

        }

    }
}
