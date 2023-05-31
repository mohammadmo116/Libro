using Libro.Application.BookTransactions.Commands;
using Libro.Application.Interfaces;
using Libro.Application.Users.Commands;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Test.BookTransactions
{
   
    public class ReserveBookCommandHandlerTest
    {
        private readonly Book _book;
        private readonly BookTransaction _bookTransaction;
        private readonly ReserveBookCommand _command;
        private readonly ReserveBookCommandHandler _handler;
        private readonly Mock<IBookTransactionRepository> _bookTransactionRepository;
        private readonly Mock<IBookRepository> _bookRepository;
        private readonly Mock<ILogger<ReserveBookCommandHandler>> _loggerMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        public ReserveBookCommandHandlerTest()
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
        
            _command = new ReserveBookCommand(_bookTransaction);

            _handler = new ReserveBookCommandHandler(
               _loggerMock.Object,
               _bookTransactionRepository.Object,
               _bookRepository.Object,
               _unitOfWorkMock.Object
               );
            
        }

        [Fact]
        public async Task Handle_Should_ReturnTrue_WhenReserveBook()
        {
            //Arrange
            _bookRepository.Setup(
                x => x.GetBookAsync(
                    It.IsAny<Guid>()))
                .ReturnsAsync(() => _book);

            _book.IsAvailable= true;
           
            _unitOfWorkMock.Setup(
               x => x.BeginTransactionAsync())
               .ReturnsAsync(It.IsAny<IDbContextTransaction>);

            _bookRepository.Setup(
              x => x.MakeBookNotAvailable(
                  It.IsAny<Book>()));

            _bookTransactionRepository.Setup(
             x => x.AddBookTransactionWithReservedStatus(
                 It.IsAny<BookTransaction>()));

            _unitOfWorkMock.Setup(
               x => x.CommitAsync(It.IsAny<IDbContextTransaction>()));

            _unitOfWorkMock.Setup(
               x => x.SaveChangesAsync())
               .ReturnsAsync(1);
            //Act
            var result = await _handler.Handle(_command, default);

            //Assert
            _bookRepository.Verify(
                  x => x.GetBookAsync(
                      It.Is<Guid>(a => a == _bookTransaction.BookId)),
                  Times.Once);


            Assert.True(_book.IsAvailable);

            _unitOfWorkMock.Verify(
               x => x.BeginTransactionAsync(),
               Times.Once);


            _bookRepository.Verify(
              x => x.MakeBookNotAvailable(
                  It.Is<Book>(a=>a==_book)),
              Times.Once);

            _bookTransactionRepository.Verify(
             x => x.AddBookTransactionWithReservedStatus(
                 It.Is<BookTransaction>(a=>a==_bookTransaction)),
             Times.Once);

            _unitOfWorkMock.Verify(
               x => x.CommitAsync(It.IsAny<IDbContextTransaction>()),
               Times.Once);

            _unitOfWorkMock.Verify(
               x => x.SaveChangesAsync(),
               Times.Once);

            Assert.True(result);
        }
        [Fact]
        public async Task Handle_Should_ThrowCustomNotFoundException_WhenBookNotFound()
        {
            //Arrange
            _bookRepository.Setup(
                x => x.GetBookAsync(
                    It.IsAny<Guid>()))
                .ReturnsAsync(() => null!);

            //Act

            async Task act() => await _handler.Handle(_command, default);
            CustomNotFoundException ActualException = await Assert.ThrowsAsync<CustomNotFoundException>(act);
            CustomNotFoundException ExpectedException = new("Book");

            //Assert
            _bookRepository.Verify(
                  x => x.GetBookAsync(
                      It.Is<Guid>(a => a == _bookTransaction.BookId)),
                  Times.Once);


            _unitOfWorkMock.Verify(
               x => x.BeginTransactionAsync(),
               Times.Never);


            _bookRepository.Verify(
              x => x.MakeBookNotAvailable(
                  It.Is<Book>(a => a == _book)),
              Times.Never);

            _bookTransactionRepository.Verify(
             x => x.AddBookTransactionWithReservedStatus(
                 It.Is<BookTransaction>(a => a == _bookTransaction)),
             Times.Never);

            _unitOfWorkMock.Verify(
               x => x.CommitAsync(It.IsAny<IDbContextTransaction>()),
               Times.Never);

            _unitOfWorkMock.Verify(
               x => x.SaveChangesAsync(),
               Times.Never);

            Assert.Equal(ExpectedException.Message, ActualException.Message);
        }
        [Fact]
        public async Task Handle_Should_ThrowBookIsNotAvailableException_WhenBookIsNotAvailable()
        {
            //Arrange
            _bookRepository.Setup(
                x => x.GetBookAsync(
                    It.IsAny<Guid>()))
                .ReturnsAsync(() => _book);

            _book.IsAvailable = false;
            //Act

            async Task act() => await _handler.Handle(_command, default);
            BookIsNotAvailableException ActualException = await Assert.ThrowsAsync<BookIsNotAvailableException>(act);
            BookIsNotAvailableException ExpectedException = new(_book.Title);

            //Assert
            _bookRepository.Verify(
                  x => x.GetBookAsync(
                      It.Is<Guid>(a => a == _bookTransaction.BookId)),
                  Times.Once);


            Assert.False( _book.IsAvailable);


            _unitOfWorkMock.Verify(
               x => x.BeginTransactionAsync(),
               Times.Never);


            _bookRepository.Verify(
              x => x.MakeBookNotAvailable(
                  It.Is<Book>(a => a == _book)),
              Times.Never);

            _bookTransactionRepository.Verify(
             x => x.AddBookTransactionWithReservedStatus(
                 It.Is<BookTransaction>(a => a == _bookTransaction)),
             Times.Never);

            _unitOfWorkMock.Verify(
               x => x.CommitAsync(It.IsAny<IDbContextTransaction>()),
               Times.Never);

            _unitOfWorkMock.Verify(
               x => x.SaveChangesAsync(),
               Times.Never);

            Assert.Equal(ExpectedException.Message, ActualException.Message);
        }
    }
}
