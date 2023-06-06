using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.Dtos.BookReview
{
    public class BookReviewDto
    {
        [Range(1, 5)]
        [Required]
        public short Rate { get; set; }
        public string? Review { get; set; }
     
    }
}
