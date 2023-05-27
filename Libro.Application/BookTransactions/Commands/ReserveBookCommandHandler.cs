using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.BookTransactions.Commands
{
    public sealed class ReserveBookCommandHandler : IRequestHandler<ReserveBookCommand>
    {
        private readonly ILogger<BookTransaction> _logger;
        private readonly IBookTransactionRepository _bookTransactionRepository;

       public ReserveBookCommandHandler(ILogger<BookTransaction> logger,
           IBookTransactionRepository bookTransactionRepository) {
            _logger = logger;
            _bookTransactionRepository = bookTransactionRepository;

        }
        public async Task Handle(ReserveBookCommand request, CancellationToken cancellationToken)
        {
            try
            {
               await _bookTransactionRepository.ReserveBookAsync(request.bookTransaction);
            }
            catch (CustomNotFoundException e)
            {
                _logger.LogInformation($"CustomNotFoundException message:{e.Message}");
                _logger.LogInformation($"bookId : {request.bookTransaction.BookId}");
                throw e;
            }
            catch (BookIsNotAvailableException e)
            {
                _logger.LogInformation($"BookIsNotAvailableException message:{e.Message}");
                _logger.LogInformation($"bookId : {request.bookTransaction.BookId}");
                throw e;
            }
        }
    }

}
