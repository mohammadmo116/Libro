using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<string>> Search(string? Title, string? AuthorName, string? Genre)
        {
            List<Book> Books = null;

            if (AuthorName is not null)
            {
                var AuthorIds = await _context.Authors.Where(b => b.Name.Contains(AuthorName)).Select(a => a.Id).ToListAsync();
                var BookIds = await _context.AuthorBooks.Where(a => AuthorIds.Contains(a.AuthorId)).Select(a => a.BookId).ToListAsync();
                Books = await _context.Books.Where(b => BookIds.Contains(b.Id)).ToListAsync();
            }
            if (Title is not null)
            {
                if (Books is null)
                    Books = await _context.Books.Where(b => b.Title.Contains(Title)).ToListAsync();
                else
                    Books = Books.Where(b => b.Title.Contains(Title)).ToList();
            }
            if (Genre is not null)
            {
                if (Books is null)
                    Books = await _context.Books.Where(b => b.Genre.Contains(Genre)).ToListAsync();
                else
                    Books = Books.Where(b => b.Genre.Contains(Genre)).ToList();

            }
            if (Books is null)
                Books = new();
            return Books.Select(a => a.Title).ToList();
        }


    }
}
