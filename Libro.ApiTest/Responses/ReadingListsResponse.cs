using Libro.Presentation.Dtos.ReadingList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.ApiTest.Responses
{
    public class ReadingListsResponse
    {
        public List<GetReadingListDto> ReadingLists { get; set; } = new();
        public int Pages { get; set; } = 0;
    }
}
