namespace Libro.Application.Interfaces
{
    public interface IBookRepository
    {
        Task<List<string>> GetBooks(string? Title, string? AuthorName, string? Genre);
    }
}