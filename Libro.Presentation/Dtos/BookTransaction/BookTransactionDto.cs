
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Presentation.Dtos.Book;
using Libro.Presentation.Dtos.User;
using System.ComponentModel.DataAnnotations;


namespace Libro.Presentation.Dtos.BookTransaction
{
    public class BookTransactionDto
    {
        public BookWithAuthorsDto Book { get; set; } = null!;
        public UserDtoWithId User { get; set; } = null!;
        public DateTime? DueDate { get; set; }
    }
}
