﻿using Libro.Application.Interfaces;
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

        public async Task AddBookTransactionWithReservedStatus(BookTransaction bookTransaction)
        {
            bookTransaction.Id = Guid.NewGuid();
            bookTransaction.Status = BookStatus.Reserved;   
            await _context.BookTransactions.AddAsync(bookTransaction);
        }


        public void ChangeBookTransactionStatusToBorrowed(BookTransaction bookTransaction, DateTime DueDate)
        {   
            bookTransaction.Status = BookStatus.Borrowed;
            bookTransaction.DueDate = DueDate;
            _context.BookTransactions.Update(bookTransaction);

        }
        public void ChangeBookTransactionStatusToNone(BookTransaction bookTransaction)
        {
                 bookTransaction.Status = BookStatus.None;
                _context.BookTransactions.Update(bookTransaction);
        }
        public void DeleteBookTransaction(BookTransaction Transaction, Book book)
        {
            _context.BookTransactions.Remove(Transaction);
        }
        public async Task<BookTransaction> GetBookTransactionByIdWhereStatusNotNone(Guid TransactionId)
        {
            return await _context.BookTransactions
                            .Where(a => a.Status != BookStatus.None)
                            .FirstOrDefaultAsync(a => a.Id == TransactionId);

        }

        public async Task<List<BookTransaction>> TrackDueDateAsync(int PageNumber,int Count)
        {
            return await _context.BookTransactions
                .Include(a=>a.User)
                .Include(a=>a.Book)
                .Where(a => a.Status == BookStatus.Borrowed)
                .OrderByDescending(a=>a.DueDate)
                .Skip(PageNumber*Count)
                .Take(Count)
                .ToListAsync();
            
        }

    }
}
