using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Exceptions;
using Libro.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.BookTransactions.Commands
{
    public class ReturnBookCommandHandler : IRequestHandler<ReturnBookCommand,bool>
    {

        private readonly ILogger<ReturnBookCommandHandler> _logger;
        private readonly IBookTransactionRepository _bookTransactionRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ReturnBookCommandHandler(ILogger<ReturnBookCommandHandler> logger,
            IBookTransactionRepository bookTransactionRepository,
            IBookRepository bookRepository,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _bookTransactionRepository = bookTransactionRepository;
            _bookRepository= bookRepository;
            _unitOfWork= unitOfWork;
    }
        public async Task<bool> Handle(ReturnBookCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            /*var BookTransaction = await _bookTransactionRepository
                  .GetBookTransactionWhereStatusNotNone(request.TransactionId);

            if (BookTransaction is null)
            {
                _logger.LogInformation($"CustomNotFoundException (BookTransaction)");
                _logger.LogInformation($"TransactionId : {request.TransactionId}");
                throw new CustomNotFoundException("BookTransaction");
            }

            var book = await _bookRepository.GetBookAsync(BookTransaction.BookId);
            if (book is null)
            {
                _logger.LogInformation($"CustomNotFoundException (Book)");
                _logger.LogInformation($"TransactionId : {request.TransactionId}");
                throw new CustomNotFoundException("Book");
            }

            var transaction = await _unitOfWork.BeginTransactionAsync();
             _bookRepository.MakeBookAvailable(book);

            if (BookTransaction.Status == BookStatus.Reserved)
            {   
                     _bookTransactionRepository
                        .DeleteBookTransaction(BookTransaction, book);
            }

            if (BookTransaction.Status == BookStatus.Borrowed)
            {
 
                 _bookTransactionRepository.ChangeBookTransactionStatusToNone(BookTransaction);
            }

            var numberOfRows = await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync(transaction);

            return numberOfRows > 0;*/

        }
    }
}
