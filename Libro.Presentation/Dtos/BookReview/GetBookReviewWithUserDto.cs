using Libro.Presentation.Dtos.User;

namespace Libro.Presentation.Dtos.BookReview
{
    public class GetBookReviewWithUserDto : GetBookReviewDto
    {
        public UserWithIdAndNameDto User { get; set; }
    }
}
