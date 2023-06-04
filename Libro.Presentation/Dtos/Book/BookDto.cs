using Libro.Presentation.Dtos.Author;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.Dtos.Book
{
    public class BookDto 
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(256)]
        public string Title { get; set; }
        public string? Genre { get; set; }
        [Required]
        public DateTime? PublishedDate { get; set; }

    }
}
