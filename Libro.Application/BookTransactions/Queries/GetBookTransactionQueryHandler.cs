using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libro.Application.BookTransactions.Queries
{
    public class GetBookTransactionQueryHandler : IRequestHandler<GetBookTransactionQuery, BookTransaction>
    {
        private readonly ILogger<GetBookTransactionQueryHandler> _logger;
        private readonly IBookTransactionRepository _bookTransactionRepository;

        public GetBookTransactionQueryHandler(ILogger<GetBookTransactionQueryHandler> logger,
            IBookTransactionRepository bookTransactionRepository)
        {
            _logger = logger;
            _bookTransactionRepository = bookTransactionRepository;
        }

        public async Task<BookTransaction> Handle(GetBookTransactionQuery request, CancellationToken cancellationToken)
        {
            var bookTransaction = await _bookTransactionRepository.GetUserBookTransactionAsync(request.UserId, request.TransactionId);
            if (bookTransaction is null)
            {
                _logger.LogInformation("CustomNotFoundException (BookTransaction)");
                throw new CustomNotFoundException("BookTransaction");
            }
            return bookTransaction;
        }
    }
}
