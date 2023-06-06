using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.Dtos.BookReview
{
    public class GetBookReviewDto:BookReviewDto
    {
        public Guid BookId { get; set; }
        public Guid UserId { get; set; }
    }
}
