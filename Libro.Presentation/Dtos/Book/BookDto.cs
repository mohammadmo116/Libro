using Libro.Presentation.Dtos.Author;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.Dtos.Book
{
    public class BookDto : CreateBookDto
    {
        public Guid Id { get; set; }
    
    }
}
