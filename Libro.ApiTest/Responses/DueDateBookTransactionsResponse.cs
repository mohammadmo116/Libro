using Libro.Presentation.Dtos.BookTransaction;

namespace Libro.ApiTest.Responses
{
    public class DueDateBookTransactionsResponse
    {
        public List<BookTransactionWithStatusAndIdDto> Transactions { get; set; } = new();
        public int Pages { get; set; } = 1;

    }
}
