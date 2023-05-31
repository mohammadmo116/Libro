using Libro.Domain.Entities;

namespace Libro.Application.Interfaces
{
    public interface IBookRepository
    {
        Task<Book> GetBookAsync(Guid BookId);

        Task<List<string>> GetAllBooksAsync(int PageNumber, int Count);
        Task<List<Book>> GetBooksByGenreAsync(List<Book>? Books, string Genre);
        Task<List<Book>> GetBooksByTitleAsync(List<Book>? Books, string Title);
        Task<List<Book>> GetBooksByAuthorNameAsync(List<Book> Books,string? AuthorName);
        void MakeBookNotAvailable(Book book);
        void MakeBookAvailable(Book book);
    }
}