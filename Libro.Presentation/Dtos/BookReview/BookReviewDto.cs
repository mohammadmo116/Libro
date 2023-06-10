using System.ComponentModel.DataAnnotations;

namespace Libro.Presentation.Dtos.BookReview
{
    public class BookReviewDto
    {
        [Range(1, 5)]
        [Required]
        public short Rate { get; set; }
        public string? Review { get; set; }
    }
}
