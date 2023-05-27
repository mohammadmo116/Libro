using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.BookTransactions.Commands
{
    public sealed class CheckOutBookCommandHandler : IRequestHandler<CheckOutBookCommand>
    {
        private readonly ILogger<BookTransaction> _logger;
        private readonly IBookTransactionRepository _bookTransactionRepository;


        public CheckOutBookCommandHandler(ILogger<BookTransaction> logger,
            IBookTransactionRepository bookTransactionRepository)
        {
            _logger = logger;
            _bookTransactionRepository = bookTransactionRepository;
        }
       public async Task Handle(CheckOutBookCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _bookTransactionRepository.CheckOutAsync(request.TransactionId,request.DueDate);
            }
            catch (CustomNotFoundException e)
            {
                _logger.LogInformation($"CustomNotFoundException message : {e.Message}");
                _logger.LogInformation($"TransactionId : {request.TransactionId}");
                throw e;
            }
            catch (BookIsBorrowedException e)

            {
                _logger.LogInformation($"BookIsBorrowedException message : {e.Message}");
                _logger.LogInformation($"TransactionId : {request.TransactionId}");
                throw e;
            }
        }
    }
}
