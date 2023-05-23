using Libro.Domain.Entities;

namespace Libro.Application.Interfaces
{
    public interface IBookRepository
    {
        Task<List<string>> GetBooksAsync(string? Title, string? AuthorName, string? Genre);
        Task<Book> GetBookAsync(Guid BookId);
    }
}