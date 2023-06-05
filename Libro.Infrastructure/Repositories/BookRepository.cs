using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace Libro.Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<string>> GetAllBooksAsync(int PageNumber, int Count)
        {
            return await _context.Books.Where(b => b.IsAvailable == true).Select(b => b.Title).Skip(PageNumber * Count).Take(Count).ToListAsync();
          //TODO 
       
        }
        public async Task<Book> GetBookAsync(Guid BookId)
        {
            return await _context.Books.Include(a => a.Authors).FirstOrDefaultAsync(b => b.Id == BookId);
        }

        public async Task CreateBookAsync(Book book)
        {
             _context.Books.AddAsync(book);
        }

        public void UpdateBook(Book book)
        {
            _context.Books.Update(book);
        }
        public void RemoveBook(Book book)
        {
            _context.Books.Remove(book);
        }

        public async Task<List<Book>> GetBooksByAuthorNameAsync(List<Book> Books, string? AuthorName)
        {
            var AuthorIds = await _context.Authors.Where(b => b.Name.Contains(AuthorName)).Select(a => a.Id).ToListAsync();
            var BookIds = await _context.AuthorBooks.Where(a => AuthorIds.Contains(a.AuthorId)).Select(a => a.BookId).ToListAsync();
            Books = await _context.Books.Where(b => BookIds.Contains(b.Id)).ToListAsync();
            return Books;
        }

        public async Task<List<Book>> GetBooksByGenreAsync(List<Book>? Books, string Genre)
        {
            if (Books is null)
                Books = await _context.Books.Where(b => b.Genre.Contains(Genre)).ToListAsync();
            else
                Books = Books.Where(b => b.Genre.Contains(Genre)).ToList();
            return Books;
        }

        public async Task<List<Book>> GetBooksByTitleAsync(List<Book>? Books, string Title)
        {
            if (Books is null)
                Books = await _context.Books.Where(b => b.Title.Contains(Title)).ToListAsync();
            else
                Books = Books.Where(b => b.Title.Contains(Title)).ToList();
            return Books;
        }
        public void MakeBookNotAvailable(Book book)
        {
            book.IsAvailable = false;
            _context.Books.Update(book);
        }
        public void MakeBookAvailable(Book book)
        {
            book.IsAvailable = true;
            _context.Books.Update(book);
        }
    
    }
}
