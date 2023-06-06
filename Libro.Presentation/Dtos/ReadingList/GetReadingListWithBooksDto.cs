using Libro.Presentation.Dtos.Book;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.Dtos.ReadingList
{
    public class GetReadingListWithBooksDto :ReadingListDto
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(250)]
   
        public List<BookDto>? Books { get; set; }
      
    }
}
