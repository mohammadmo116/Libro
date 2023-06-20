using Libro.Presentation.Dtos.Book;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.ApiTest.Responses
{
    public class RecommendedBooksResponse
    {
        public List<BookWithAuthorsDto> RecommendedBooks { get; set; } = new();
        public int Pages { get; set; } = 0;
    }
}
