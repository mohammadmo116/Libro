using Libro.Domain.Entities;
using Libro.Presentation.Dtos.Author;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.Dtos.Book
{
    public class BookWithAuthorsDto : BookDto
    {
        public Guid Id { get; set; }
  
        public List<AuthorDto>? Authors { get; set; }

    }
}
