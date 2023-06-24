using Libro.Presentation.Dtos.BookReview;

namespace Libro.ApiTest.Responses
{
    public class BookReviewsResponse
    {
        public List<GetBookReviewWithUserDto> Reviews { get; set; } = new();
        public int Pages { get; set; } = 0;
    }
}
