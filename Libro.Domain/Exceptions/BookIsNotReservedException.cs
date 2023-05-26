using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Exceptions
{
    public class BookIsNotReservedException :Exception

    { 
        public BookIsNotReservedException(string title) : base($"the Requested book {title} is not reserved, please reserve the book first")
        { 
        }
    }
}
