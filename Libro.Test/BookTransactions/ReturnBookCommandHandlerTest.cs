using Libro.Application.BookTransactions.Commands;
using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Exceptions;
using Libro.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Test.BookTransactions
{
    public class ReturnBookCommandHandlerTest
    {
        private readonly Book _book;
        private readonly BookTransaction _bookTransaction;
        private readonly ReturnBookCommand _command;
        private readonly ReturnBookCommandHandler _handler;
        private readonly Mock<IBookTransactionRepository> _bookTransactionRepository;
        private readonly Mock<IBookRepository> _bookRepository;
        private readonly Mock<ILogger<ReturnBookCommandHandler>> _loggerMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;


        public ReturnBookCommandHandlerTest()
        {

            _bookTransactionRepository = new();
            _bookRepository = new();
            _loggerMock = new();
            _unitOfWorkMock = new();

            _book = new()
            {
                Id = Guid.NewGuid(),
                Title = "title",
                IsAvailable = true,
                Genre = "genre"
            };
            _bookTransaction = new()
            {
                Id = Guid.NewGuid(),
                BookId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
            };

            _command = new ReturnBookCommand(_bookTransaction.Id);

            _handler = new ReturnBookCommandHandler(
               _loggerMock.Object,
               _bookTransactionRepository.Object,
               _bookRepository.Object,
               _unitOfWorkMock.Object
               );
        }
     
        /////Handle_Should_ReturnTrue_WhenReturnBookWhenStatusIsReservedOrBorrowed
        [Theory]
        [InlineData(BookStatus.Reserved)]
        [InlineData(BookStatus.Borrowed)]
        public async Task Handle_Should_ReturnTrue_WhenReturnBookWhenStatusIsReservedOrBorrowed(
            BookStatus status)
        {
            //Arrange
            _bookTransactionRepository.Setup(
              x => x.GetBookTransactionWhereStatusNotNone(
                         It.IsAny<Guid>())
              )
              .ReturnsAsync(() => _bookTransaction);

            _bookRepository.Setup(
              x => x.GetBookAsync(
                  It.IsAny<Guid>()))
              .ReturnsAsync(() => _book);


            _unitOfWorkMock.Setup(
            x => x.BeginTransactionAsync())
               .ReturnsAsync(It.IsAny<IDbContextTransaction>());

            _bookRepository.Setup(
              x => x.MakeBookAvailable(
                  It.IsAny<Book>()));

            _bookTransaction.Status = status;

            if (_bookTransaction.Status == BookStatus.Reserved)
            {
                _bookTransactionRepository.Setup(
                 x => x.DeleteBookTransaction(
                     It.IsAny<BookTransaction>(),
                     It.IsAny<Book>()));
            }

            if (_bookTransaction.Status == BookStatus.Borrowed)
            {
                _bookTransactionRepository.Setup(
                  x => x.ChangeBookTransactionStatusToNone(
                      It.IsAny<BookTransaction>()
                      ));
            }

            _unitOfWorkMock.Setup(
               x => x.SaveChangesAsync())
               .ReturnsAsync(2);

            _unitOfWorkMock.Setup(
               x => x.CommitAsync(It.IsAny<IDbContextTransaction>()
               ));

            
            //Act
            var result = await _handler.Handle(_command, default);

            //Assert
            Assert.True(result);

            _bookTransactionRepository.Verify(
          x => x.GetBookTransactionWhereStatusNotNone(
               It.Is<Guid>(a => a == _bookTransaction.Id)
                 ),
          Times.Once);


            _bookRepository.Verify(
              x => x.GetBookAsync(
                  It.Is<Guid>(a => a == _bookTransaction.BookId)),
              Times.Once);


            _unitOfWorkMock.Verify(
            x => x.BeginTransactionAsync(),
            Times.Once);

            _bookRepository.Verify(
              x => x.MakeBookAvailable(
                  It.Is<Book>(a=>a==_book)),
              Times.Once);


            if (_bookTransaction.Status == BookStatus.Reserved)
            {
                Assert.Equal(BookStatus.Reserved,_bookTransaction.Status);
                _bookTransactionRepository.Verify(
                 x => x.DeleteBookTransaction(
                     It.Is<BookTransaction>(a=>a==_bookTransaction),
                     It.Is<Book>(a=>a==_book)),
                 Times.Once);
            }

            if (_bookTransaction.Status == BookStatus.Borrowed)
            {
                Assert.Equal(BookStatus.Borrowed, _bookTransaction.Status);
                _bookTransactionRepository.Verify(
                  x => x.ChangeBookTransactionStatusToNone(
                      It.Is<BookTransaction>(a=>a==_bookTransaction)),
                  Times.Once);
            }

            _unitOfWorkMock.Verify(
               x => x.SaveChangesAsync(),
               Times.Once);

            _unitOfWorkMock.Verify(
               x => x.CommitAsync(It.IsAny<IDbContextTransaction>()),
               Times.Once);

        }


        /////////Handle_Should_ThrowCustomNotFoundException_WhenBookTransactionNotFound
        [Theory]
        [InlineData(BookStatus.Reserved)]
        [InlineData(BookStatus.Borrowed)]
        [InlineData(BookStatus.None)]
        public async Task Handle_Should_ThrowCustomNotFoundException_WhenBookTransactionNotFound(
          BookStatus status)
        {
            //Arrange
            _bookTransactionRepository.Setup(
              x => x.GetBookTransactionWhereStatusNotNone(
                    It.IsAny<Guid>())
              )
              .ReturnsAsync(() => null!);


            //Act
            async Task act() => await _handler.Handle(_command, default);
            CustomNotFoundException ActualException = await Assert.ThrowsAsync<CustomNotFoundException>(act);
            CustomNotFoundException ExpectedException = new("BookTransaction");

            //Assert

            Assert.Equal(ExpectedException.Message, ActualException.Message);

            _bookTransactionRepository.Verify(
          x => x.GetBookTransactionWhereStatusNotNone(
                 It.Is<Guid>(a => a == _bookTransaction.Id)
                 ),
          Times.Once);


            _bookRepository.Verify(
              x => x.GetBookAsync(
                  It.Is<Guid>(a => a == _bookTransaction.BookId)),
              Times.Never);


            _unitOfWorkMock.Verify(
            x => x.BeginTransactionAsync(),
            Times.Never);

            _bookRepository.Verify(
              x => x.MakeBookAvailable(
                  It.Is<Book>(a => a == _book)),
              Times.Never);
    
                _bookTransactionRepository.Verify(
                 x => x.DeleteBookTransaction(
                     It.Is<BookTransaction>(a => a == _bookTransaction),
                     It.Is<Book>(a => a == _book)),
                 Times.Never);  
           
                _bookTransactionRepository.Verify(
                  x => x.ChangeBookTransactionStatusToNone(
                      It.Is<BookTransaction>(a => a == _bookTransaction)),
                  Times.Never);


            _unitOfWorkMock.Verify(
               x => x.SaveChangesAsync(),
               Times.Never);

            _unitOfWorkMock.Verify(
               x => x.CommitAsync(It.IsAny<IDbContextTransaction>()),
               Times.Never);



        }


        /////////Handle_Should_ThrowCustomNotFoundException_WhenBookNotFound
        [Theory]
        [InlineData(BookStatus.Reserved)]
        [InlineData(BookStatus.Borrowed)]
        [InlineData(BookStatus.None)]
        public async Task Handle_Should_ThrowCustomNotFoundException_WhenBookNotFound(
         BookStatus status)
        {
            //Arrange
            _bookTransactionRepository.Setup(
              x => x.GetBookTransactionWhereStatusNotNone(
                       It.IsAny<Guid>())
              )
              .ReturnsAsync(() => _bookTransaction);

            _bookRepository.Setup(
             x => x.GetBookAsync(
                 It.IsAny<Guid>()))
             .ReturnsAsync(() => null!);
            //Act
            async Task act() => await _handler.Handle(_command, default);
            CustomNotFoundException ActualException = await Assert.ThrowsAsync<CustomNotFoundException>(act);
            CustomNotFoundException ExpectedException = new("Book");

            //Assert

            Assert.Equal(ExpectedException.Message, ActualException.Message);

            _bookTransactionRepository.Verify(
          x => x.GetBookTransactionWhereStatusNotNone(
                  It.Is<Guid>(a => a == _bookTransaction.Id)
                 ),
          Times.Once);


            _bookRepository.Verify(
              x => x.GetBookAsync(
                  It.Is<Guid>(a => a == _bookTransaction.BookId)),
              Times.Once);


            _unitOfWorkMock.Verify(
            x => x.BeginTransactionAsync(),
            Times.Never);

            _bookRepository.Verify(
              x => x.MakeBookAvailable(
                  It.Is<Book>(a => a == _book)),
              Times.Never);

            _bookTransactionRepository.Verify(
             x => x.DeleteBookTransaction(
                 It.Is<BookTransaction>(a => a == _bookTransaction),
                 It.Is<Book>(a => a == _book)),
             Times.Never);

            _bookTransactionRepository.Verify(
              x => x.ChangeBookTransactionStatusToNone(
                  It.Is<BookTransaction>(a => a == _bookTransaction)),
              Times.Never);


            _unitOfWorkMock.Verify(
               x => x.SaveChangesAsync(),
               Times.Never);

            _unitOfWorkMock.Verify(
               x => x.CommitAsync(It.IsAny<IDbContextTransaction>()),
               Times.Never);



        }

    }
}
