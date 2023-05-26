using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Exceptions
{
    public class BookIsNotReservedOrBorrowedException : Exception
    {
        public BookIsNotReservedOrBorrowedException(string title) :base($"Book {title} is not Reserved or Borrowed")
        { 
        }
    }
}
