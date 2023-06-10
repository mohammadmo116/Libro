using System.ComponentModel.DataAnnotations;

namespace Libro.Domain.Entities
{
    public class BookReview
    {

        [Range(1, 5)]
        [Required]
        public short Rate { get; set; }
        public string? Review { get; set; }

        public Guid BookId { get; set; }
        public Book Book { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
