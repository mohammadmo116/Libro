using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.Dtos.Book
{
    public class GetBookDto : BookDto
    {
        public Guid Id { get; set; }
    }
}
