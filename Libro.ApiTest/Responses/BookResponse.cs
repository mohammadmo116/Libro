using Libro.Presentation.Dtos.Book;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.ApiTest.Responses
{
    public class BookResponse
    {
        public List<GetBookDto> books { get; set; }=new ();
        public int pages { get; set; } = 0;
    }
}
