using Libro.Presentation.Dtos.Book;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.Dtos.ReadingList
{
    public class ReadingListWithBooksDto : ReadingListDto
    {
        public List<BookDto>? Books { get; set; }

    }
}
