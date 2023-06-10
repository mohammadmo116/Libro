using System.ComponentModel.DataAnnotations;

namespace Libro.Presentation.Dtos.Book
{
    public class BookDto
    {

        [Required]
        [MaxLength(256)]
        public string Title { get; set; }
        public string? Genre { get; set; }
        [Required]
        public DateTime? PublishedDate { get; set; }

    }
}
