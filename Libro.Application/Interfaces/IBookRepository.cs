using Libro.Domain.Entities;

namespace Libro.Application.Interfaces
{
    public interface IBookRepository
    {
        Task<Book> GetBookAsync(Guid BookId);
        Task<(List<Book>, int)> GetAllBooksAsync(int PageNumber, int Count);
        Task CreateBookAsync(Book book);
        void UpdateBook(Book book);
        void RemoveBook(Book book);
        Task<(List<Book>, int)> GetSearchedBooksAsync(string? Title, string? AuthorName, string? Genre, int PageNumber, int Count);
        void MakeBookNotAvailable(Book book);
        void MakeBookAvailable(Book book);
        Task CreateReviewAsync(BookReview bookReview);
        Task<bool> BookIsReviewedByUser(Guid UserId, Guid BookId);
    }
}