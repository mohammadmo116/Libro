using Libro.Application.Interfaces;
using Libro.Domain.Entities;
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

        public async Task<Book> GetBookAsync(Guid BookId)
        {
            return await _context.Books.Include(a => a.Authors).FirstOrDefaultAsync(b => b.Id == BookId);
        }

        public async Task<List<string>> GetBooksAsync(string? Title, string? AuthorName, string? Genre)
        {
            if (Title is null && AuthorName is null && Genre is null)
            {
                return await _context.Books.Where(b=>b.IsAvailable==true).Select(b => b.Title).ToListAsync(); 
            }

            List<Book> Books = null;

            if (AuthorName is not null)
            {
                var AuthorIds = await _context.Authors.Where(b => b.Name.Contains(AuthorName)).Select(a => a.Id).ToListAsync();
                var BookIds = await _context.AuthorBooks.Where(a => AuthorIds.Contains(a.AuthorId)).Select(a => a.BookId).ToListAsync();
                Books = await _context.Books.Where(b => BookIds.Contains(b.Id)).ToListAsync();
            }
            if (Title is not null)
            {
                Books=await GetBooksByTitleAsync(Books, Title);           
            }
            if (Genre is not null)
            {
                Books=await GetBooksByGenreAsync(Books, Genre);
               
            }

            Books ??= new();
            return Books.Select(a => a.Title).ToList();
        }

        private async Task<List<Book>> GetBooksByGenreAsync(List<Book>? Books, string Genre)
        {
            if (Books is null)
                Books = await _context.Books.Where(b => b.Genre.Contains(Genre)).ToListAsync();
            else
                Books = Books.Where(b => b.Genre.Contains(Genre)).ToList();
            return Books;
        }

        private async Task<List<Book>> GetBooksByTitleAsync(List<Book>? Books, string Title)
        {
            if (Books is null)
                Books = await _context.Books.Where(b => b.Title.Contains(Title)).ToListAsync();
            else
                Books = Books.Where(b => b.Title.Contains(Title)).ToList();
            return Books;
        }
    }
}
