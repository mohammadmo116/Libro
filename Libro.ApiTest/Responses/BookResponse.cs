using Libro.Presentation.Dtos.Book;

namespace Libro.ApiTest.Responses
{
    public class BookResponse
    {
        public List<GetBookDto> books { get; set; } = new();
        public int Pages { get; set; } = 0;
    }
}
