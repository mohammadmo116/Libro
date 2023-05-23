using Libro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Infrastructure.Repositories
{
    public class BookRepository
    {
        private readonly ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext context) {
            _context = context;
        }

        public async Task<List<string>> Search(string? Title, string? AuthorName, string? Genre ) 
        {
            List<string> BookNames = new ();

            if (AuthorName is not null)
            {
                var AuthorIds = await _context.Authors.Where(b => b.Name == $"%{AuthorName}%").Select(a=>a.Id).ToListAsync();
                var BookIds = await _context.AuthorBooks.Where(a => AuthorIds.Contains(a.AuthorId)).Select(a=>a.BookId).ToListAsync();
                BookNames = await _context.Books.Where(b=> BookIds.Contains(b.Id)).Select(b=>b.Title).ToListAsync();
            }
            if (Title is not null)
            {
                BookNames = await _context.Books.Where(b => b.Title == $"%{Title}%").Select(b => b.Title).ToListAsync(); 
            }
            if (Genre is not null)
            {
                BookNames = await _context.Books.Where(b => b.Genre == $"%{Genre}%").Select(b => b.Title).ToListAsync();
            }
         
            return BookNames;
        }


    }
}
