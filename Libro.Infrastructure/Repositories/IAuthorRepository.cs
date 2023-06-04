using Libro.Domain.Entities;

namespace Libro.Infrastructure.Repositories
{
    public interface IAuthorRepository
    {
        Task CreateAuthorAsync(Author author);
    }
}