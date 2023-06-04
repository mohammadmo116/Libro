using Libro.Domain.Enums;
using Libro.Presentation.Dtos.Book;
using Libro.Presentation.Dtos.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.Dtos.BookTransaction
{
    public class BookTransactionWithStatusDto
    {
        public CreateBookDto Book { get; set; } = null!;
        public UserDtoWithId User { get; set; } = null!;
        public DateTime? DueDate { get; set; }
        public BookStatus Status { get; set; }
    }
}
