using Libro.Presentation.Dtos.Book;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.Dtos.Author
{
    public class CreateAuthorDto
    {
        [Required]
        [MaxLength(256)]
        public string? Name { get; set; }
        public DateTime? DateOfBirth { get; set; }

    }
}
