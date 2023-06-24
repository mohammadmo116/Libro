using Libro.Domain.Enums;
using Libro.Presentation.Dtos.Book;

namespace Libro.Presentation.Dtos.BookTransaction
{
    public class BookTransactionWithStatusAndIdDto
    {
        public Guid Id { get; set; }
        public CreateBookDto Book { get; set; } = null!;
        public DateTime? DueDate { get; set; }
        public BookStatus Status { get; set; }

    }
}
