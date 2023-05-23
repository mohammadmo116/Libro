using Libro.Domain.Entities;

namespace Libro.Application.Interfaces
{
    public interface IBookRepository
    {
        Task<List<string>> GetBooks(string? Title, string? AuthorName, string? Genre);
        Task<Book> GetBook(Guid BookId);
    }
}