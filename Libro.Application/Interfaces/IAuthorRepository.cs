using Libro.Domain.Entities;

namespace Libro.Application.Repositories
{
    public interface IAuthorRepository
    {
        Task CreateAuthorAsync(Author author);
        Task<Author> GetAuthorAsync(Guid AuthorId);
        void UpdateAuthor(Author author);
        void RemoveAuthor(Author author);
    }
}