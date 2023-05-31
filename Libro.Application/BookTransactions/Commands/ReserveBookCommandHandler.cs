using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.BookTransactions.Commands
{
    public sealed class ReserveBookCommandHandler : IRequestHandler<ReserveBookCommand,bool>
    {
        private readonly ILogger<ReserveBookCommandHandler> _logger;
        private readonly IBookTransactionRepository _bookTransactionRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ReserveBookCommandHandler(ILogger<ReserveBookCommandHandler> logger,
           IBookTransactionRepository bookTransactionRepository,
           IBookRepository bookRepository,
           IUnitOfWork unitOfWork) {
            _logger = logger;
            _bookTransactionRepository = bookTransactionRepository;
            _bookRepository = bookRepository;
            _unitOfWork = unitOfWork;

        }
        public async Task<bool> Handle(ReserveBookCommand request, CancellationToken cancellationToken)
        {

            var book = await _bookRepository.GetBookAsync(request.bookTransaction.BookId);
            if (book is null)
            {
                _logger.LogInformation($"CustomNotFoundException(Book)");
                _logger.LogInformation($"bookId : {request.bookTransaction.BookId}");
                throw new CustomNotFoundException("Book");
            }
            if (!book.IsAvailable)
            {
                _logger.LogInformation($"BookIsNotAvailableException");
                _logger.LogInformation($"bookId : {request.bookTransaction.BookId}");
                throw new BookIsNotAvailableException(book.Title!);
            }
            var dbTransaction = await _unitOfWork.BeginTransactionAsync();
             _bookRepository.MakeBookNotAvailable(book);
            await _bookTransactionRepository.AddBookTransactionWithReservedStatus(request.bookTransaction);
            var numberOfRows=await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync(dbTransaction);
            return numberOfRows > 0;


        }
    }

}
