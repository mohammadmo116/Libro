using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Exceptions
{
    public class BookIsNotBorrowedException : Exception
    {
        public BookIsNotBorrowedException(string title) :base($"Book {title} is not Borrowed")
        { 
        }
    }
}
