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

        //private readonly ILogger<BookTransactions> _logger;

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
                await _bookTransactionRepository.CheckOut(request.UserId, request.BookId, request.dueDate);
            }
            catch (CustomNotFoundException e)
            {
                _logger.LogInformation($"CustomNotFoundException message : {e.Message}");
                _logger.LogInformation($"bookId : {request.BookId}, userId : {request.UserId}");
                throw e;
            }
            catch (BookIsNotReservedException e)
            {
                _logger.LogInformation($"BookIsNotReservedException message : {e.Message}");
                _logger.LogInformation($"bookId : {request.BookId}, userId : {request.UserId}");
                throw e;
            }
        }
    }
}
