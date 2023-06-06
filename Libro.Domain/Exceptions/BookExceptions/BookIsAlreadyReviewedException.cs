using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Exceptions.BookExceptions
{
    public class BookIsAlreadyReviewedException : Exception
    {
        public BookIsAlreadyReviewedException():base("you Already have Reviewed this book") { }
    }
}
