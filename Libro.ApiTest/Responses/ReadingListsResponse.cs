using Libro.Presentation.Dtos.ReadingList;

namespace Libro.ApiTest.Responses
{
    public class ReadingListsResponse
    {
        public List<GetReadingListDto> ReadingLists { get; set; } = new();
        public int Pages { get; set; } = 0;
    }
}
