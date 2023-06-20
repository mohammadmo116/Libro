using Libro.Presentation.Dtos.ReadingList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.ApiTest.Responses
{
    public class ReadingListWithBookResponse
    {
        public GetReadingListWithBooksDto ReadingList { get; set; } = new();
        public int Pages { get; set; } = 0;
    }
}
