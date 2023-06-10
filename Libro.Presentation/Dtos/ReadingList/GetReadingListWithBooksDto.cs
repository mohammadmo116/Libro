using Libro.Presentation.Dtos.Book;
using System.ComponentModel.DataAnnotations;

namespace Libro.Presentation.Dtos.ReadingList
{
    public class GetReadingListWithBooksDto : ReadingListDto
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(250)]

        public List<BookDto>? Books { get; set; }

    }
}
