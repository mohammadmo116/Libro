namespace Libro.Presentation.Dtos.BookReview
{
    public class GetBookReviewDto : BookReviewDto
    {
        public Guid BookId { get; set; }
        public Guid UserId { get; set; }
    }
}
