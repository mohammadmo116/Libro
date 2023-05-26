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
            bookTransaction.Status = BookStatus.Reserved;
            _context.BookTransactions.AddAsync(bookTransaction);
            await contextTransactio.CommitAsync();
        }


    }
}
