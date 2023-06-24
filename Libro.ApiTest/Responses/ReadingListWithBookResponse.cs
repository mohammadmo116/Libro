using Libro.Presentation.Dtos.ReadingList;

namespace Libro.ApiTest.Responses
{
    public class ReadingListWithBookResponse
    {
        public GetReadingListWithBooksDto ReadingList { get; set; } = new();
        public int Pages { get; set; } = 0;
    }
}
