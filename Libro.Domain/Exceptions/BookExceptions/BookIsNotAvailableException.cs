using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Exceptions.BookExceptions
{
    public class BookIsNotAvailableException : Exception

    {
        public BookIsNotAvailableException(string title) : base($"the Book {title} is not avaialble at the moment")
        {
        }
    }
}