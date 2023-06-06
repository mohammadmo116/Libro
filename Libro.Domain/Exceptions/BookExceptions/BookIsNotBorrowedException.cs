using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Exceptions.BookExceptions
{
    public class BookIsNotBorrowedException : Exception
    {
        public BookIsNotBorrowedException(string title) : base($"Book {title} is not Borrowed")
        {
        }
    }
}
