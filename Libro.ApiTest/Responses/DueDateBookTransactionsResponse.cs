using Libro.Presentation.Dtos.BookTransaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.ApiTest.Responses
{
    public class DueDateBookTransactionsResponse
    {
        public List<BookTransactionWithStatusAndIdDto> Transactions { get; set; } = new();
        public int Pages { get; set; } = 1;

    }
}
