using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Exceptions;
using Libro.Domain.Exceptions.BookExceptions;
using Libro.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.BookTransactions.Commands
{
    public sealed class CheckOutBookCommandHandler : IRequestHandler<CheckOutBookCommand,bool>
    {
        private readonly ILogger<CheckOutBookCommandHandler> _logger;
        private readonly IBookTransactionRepository _bookTransactionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CheckOutBookCommandHandler(ILogger<CheckOutBookCommandHandler> logger,
            IBookTransactionRepository bookTransactionRepository,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _bookTransactionRepository = bookTransactionRepository;
            _unitOfWork= unitOfWork;
        }
       public async Task<bool> Handle(CheckOutBookCommand request, CancellationToken cancellationToken)
        {
            
                var bookTransaction = await _bookTransactionRepository.GetBookTransactionWhereStatusNotNone(request.TransactionId);
               if (bookTransaction is null)
                {
                    _logger.LogInformation($"CustomNotFoundException (BookTransaction)");
                    _logger.LogInformation($"Transaction Id : {request.TransactionId}");
                    throw new CustomNotFoundException("bookTransaction");
                }

                if (bookTransaction.Status == BookStatus.Borrowed)
                {
                    _logger.LogInformation($"BookIsBorrowedException");
                    _logger.LogInformation($"BookTransaction : {request.TransactionId}");
                    throw new BookIsBorrowedException();
                }

                _bookTransactionRepository.ChangeBookTransactionStatusToBorrowed(bookTransaction, request.DueDate);
                var numberOfRows = await _unitOfWork.SaveChangesAsync();
                return numberOfRows > 0;
            
   
        }
    }
}
