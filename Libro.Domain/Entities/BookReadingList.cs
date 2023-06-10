using System.ComponentModel.DataAnnotations;

namespace Libro.Domain.Entities
{
    public class BookReadingList
    {
        [Required]
        public Guid BookId { get; set; }

        [Required]
        public Guid ReadingListId { get; set; }

        public Book Book { get; set; } = null!;
        public ReadingList ReadingList { get; set; } = null!;

    }
}
