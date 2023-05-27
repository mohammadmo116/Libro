using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Libro.Infrastructure.Repositories
{
    public class BookTransactionRepository : IBookTransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public BookTransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ReserveBookAsync(Domain.Entities.BookTransaction bookTransaction)
        {


            var book = await _context.Books.
                FirstOrDefaultAsync(b => b.Id == bookTransaction.BookId)
                ?? throw new CustomNotFoundException("Book");
            if (!book.IsAvailable)
            { throw new BookIsNotAvailableException(book.Title); }
            using var contextTransactio = await _context.Database.BeginTransactionAsync();
            book.IsAvailable = false;
            _context.Books.Update(book);
            bookTransaction.Id = Guid.NewGuid();
            bookTransaction.Status = BookStatus.Reserved;   
            _context.BookTransactions.AddAsync(bookTransaction);
            await _context.SaveChangesAsync();
            await contextTransactio.CommitAsync();
        }
        public async Task CheckOutAsync(Guid TransactionId)
        {

            var transaction = await _context.BookTransactions
                .Where(a => a.Status != BookStatus.None)
                .FirstOrDefaultAsync(a => a.Id == TransactionId)
                ?? throw new CustomNotFoundException("bookTransaction");

            if (transaction.Status == BookStatus.Borrowed)
                throw new BookIsBorrowedException();

            transaction.Status = BookStatus.Borrowed;

            _context.BookTransactions.Update(transaction);
            await _context.SaveChangesAsync();

        }

        public async Task ReturnBookAsync(Guid TransactionId)
        {
            var transaction = await _context.BookTransactions
                .Where(a=>a.Status!=BookStatus.None)
                .FirstOrDefaultAsync(a=>a.Id== TransactionId)
               ?? throw new CustomNotFoundException("bookTransaction");
            
            var book = await _context.Books.FindAsync(transaction.BookId)
                ??throw new CustomNotFoundException("Book");
   
            if (transaction.Status == BookStatus.Reserved)
            {
               await DeleteBookTransactionAsync(transaction, book);
               return;

            }
            if (transaction.Status == BookStatus.Borrowed)
            {
                using var contextTransactio = await _context.Database.BeginTransactionAsync();
                book.IsAvailable = true;
                _context.Books.Update(book);
                transaction.Status = BookStatus.None;
                _context.BookTransactions.Update(transaction);
                await _context.SaveChangesAsync();
                await contextTransactio.CommitAsync();
            }
   
      

        }
   
        private async Task DeleteBookTransactionAsync(BookTransaction Transaction,Book book) {

            using var contextTransactio = await _context.Database.BeginTransactionAsync();

            book.IsAvailable = true;
            _context.Books.Update(book);
            _context.BookTransactions.Remove(Transaction);
            await _context.SaveChangesAsync();

            await contextTransactio.CommitAsync();


        }
        public async Task<List<BookTransaction>> TrackDueDate()
        {
            return await _context.BookTransactions.Include(a=>a.User).Include(a=>a.Book)
                .Where(a => a.Status == BookStatus.Borrowed).OrderByDescending(a=>a.DueDate).ToListAsync();

        }

    }
}
