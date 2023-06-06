using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Exceptions.BookExceptions
{
    public class BookIsBorrowedException : Exception
    {
        public BookIsBorrowedException() : base("book is already Borrowed")
        {
        }

    }
}
