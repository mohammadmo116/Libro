using Libro.Presentation.Dtos.Book;

namespace Libro.ApiTest.Responses
{
    public class RecommendedBooksResponse
    {
        public List<BookWithAuthorsDto> RecommendedBooks { get; set; } = new();
        public int Pages { get; set; } = 0;
    }
}
