﻿using Libro.Presentation.Dtos.Book;
using Libro.Presentation.Dtos.User;


namespace Libro.Presentation.Dtos.BookTransaction
{
    public class BookTransactionDto
    {
        public CreateBookDto Book { get; set; } = null!;
        public UserDtoWithId User { get; set; } = null!;
        public DateTime? DueDate { get; set; }
    }
}
