using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Infrastructure.Repositories
{
    public class BookTransactionRepository : IBookTransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public BookTransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ReserveBook(BookTransaction bookTransaction)
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
        public async Task CheckOut(Guid UserId,Guid BookId,DateTime dueDate)
        {

        
            var book = await _context.Books.
                FirstOrDefaultAsync(b => b.Id == BookId)
                ?? throw new CustomNotFoundException("Book");

            var bookTransaction = _context.BookTransactions
            .FirstOrDefault(BookIsReserved(UserId,BookId))
            ?? throw new BookIsNotAvailableException(book.Title);

            bookTransaction.Status = BookStatus.Borrowed;
            bookTransaction.DueDate= dueDate;
            _context.BookTransactions.Update(bookTransaction);
            await _context.SaveChangesAsync();
            
        }

        private static Func<BookTransaction, bool> BookIsReserved(Guid UserId,Guid BookId)
        {
            return a => a.UserId == UserId && a.BookId == BookId && a.Status == BookStatus.Reserved;
           
        }
    }
}
