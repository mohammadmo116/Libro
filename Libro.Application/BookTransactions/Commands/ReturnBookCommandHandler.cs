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
    public class ReturnBookCommandHandler : IRequestHandler<ReturnBookCommand>
    {

        private readonly ILogger<BookTransaction> _logger;
        private readonly IBookTransactionRepository _bookTransactionRepository;

        public ReturnBookCommandHandler(ILogger<BookTransaction> logger,
            IBookTransactionRepository bookTransactionRepository)
        {
            _logger = logger;
            _bookTransactionRepository = bookTransactionRepository;

        }
        public async Task Handle(ReturnBookCommand request, CancellationToken cancellationToken)
        {

            try
            {
               await _bookTransactionRepository.ReturnBookAsync(request.TransactionId);

            }

            catch (CustomNotFoundException e)
            {
                _logger.LogInformation($"CustomNotFoundException message:{e.Message}");
                _logger.LogInformation($"TransactionId : {request.TransactionId}");
                throw e;
            }
       
        }
    }
}
