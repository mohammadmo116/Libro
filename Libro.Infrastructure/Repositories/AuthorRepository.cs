using Libro.Application.Repositories;
using Libro.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Libro.Infrastructure.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {

        private readonly ApplicationDbContext _context;

        public AuthorRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Author> GetAuthorAsync(Guid AuthorId)
        {
            return await _context.Authors.FirstOrDefaultAsync(b => b.Id == AuthorId);


        }
        public async Task CreateAuthorAsync(Author author)
        {
            _context.Authors.AddAsync(author);
        }
        public void UpdateAuthor(Author author)
        {
            _context.Authors.Update(author);
        }
        public void RemoveAuthor(Author author)
        {
            _context.Authors.Remove(author);
        }
    }
}
