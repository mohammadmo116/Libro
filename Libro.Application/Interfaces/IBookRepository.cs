namespace Libro.Application.Interfaces
{
    public interface IBookRepository
    {
        Task<List<string>> Search(string? Title, string? AuthorName, string? Genre);
    }
}