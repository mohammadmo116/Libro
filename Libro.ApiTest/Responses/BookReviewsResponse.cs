using Libro.Presentation.Dtos.Book;
using Libro.Presentation.Dtos.BookReview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.ApiTest.Responses
{
    public class BookReviewsResponse
    {
        public List<GetBookReviewWithUserDto> Reviews { get; set; } = new();
        public int pages { get; set; } = 0;
    }
}
