using Libro.Application.BookTransactions.Commands;
using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Exceptions;
using Libro.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;

namespace Libro.Test.BookTransactions
{
    public class CheckOutBookCommandHandlerTest
    {  
        private readonly BookTransaction _bookTransaction;
        private readonly CheckOutBookCommand _command;
        private readonly CheckOutBookCommandHandler _handler;
        private readonly Mock<IBookTransactionRepository> _bookTransactionRepository;
        private readonly Mock<ILogger<CheckOutBookCommandHandler>> _loggerMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
       
        public CheckOutBookCommandHandlerTest()
        {

            _bookTransactionRepository = new();
            _loggerMock = new();
            _unitOfWorkMock = new();

           
            _bookTransaction = new()
            {
                Id= Guid.NewGuid(),
                BookId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
            };

            _command = new CheckOutBookCommand(_bookTransaction.Id, DateTime.UtcNow.AddDays(1));

            _handler = new CheckOutBookCommandHandler(
               _loggerMock.Object,
               _bookTransactionRepository.Object,
               _unitOfWorkMock.Object
               );
        }

        [Fact]
        public async Task Handle_Should_ReturnTrue_WhenBorrowBook()
        {
            //Arrange
            _bookTransactionRepository.Setup(
                x => x.GetBookTransactionWhereStatusNotNone(
                    It.IsAny<Guid>())
                )
                .ReturnsAsync(() => _bookTransaction);

             _bookTransaction.Status = BookStatus.Reserved;

            _bookTransactionRepository.Setup(
                x => x.ChangeBookTransactionStatusToBorrowed(
                    It.IsAny<BookTransaction>(),
                    It.Is<DateTime>(a => a > DateTime.UtcNow)
                    )
                );

            _unitOfWorkMock.Setup(
             x => x.SaveChangesAsync()
             ).ReturnsAsync(1);

            //Act
            var result = await _handler.Handle(_command, default);


            //Assert
            _bookTransactionRepository.Verify(
               x => x.GetBookTransactionWhereStatusNotNone(
                   It.Is<Guid>(a => a == _bookTransaction.Id)
                   ),
               Times.Once);

            Assert.Equal(BookStatus.Reserved,_bookTransaction.Status);

            _bookTransactionRepository.Verify(
                x => x.ChangeBookTransactionStatusToBorrowed(
                    It.Is<BookTransaction>(a=>a==_bookTransaction),
                    It.Is<DateTime>(a => a > DateTime.UtcNow)
                    ),
                Times.Once
                );

            _unitOfWorkMock.Verify(
             x => x.SaveChangesAsync(), 
             Times.Once);
            Assert.True(result);
        }

        [Fact]
        public async Task Handle_Should_ThrowCustomNotFoundException_WhenBookTransactionNotFound()
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
            CustomNotFoundException ExpectedException = new("bookTransaction");

            //Assert
            _bookTransactionRepository.Verify(
               x => x.GetBookTransactionWhereStatusNotNone(
                   It.Is<Guid>(a => a == _bookTransaction.Id)
                   ),
               Times.Once);

            _bookTransactionRepository.Verify(
               x => x.ChangeBookTransactionStatusToBorrowed(
                   It.IsAny<BookTransaction>(),
                   It.IsAny<DateTime>()
                   ),
               Times.Never
               );

            _unitOfWorkMock.Verify(
             x => x.SaveChangesAsync(),
             Times.Never);

            Assert.Equal(ExpectedException.Message, ActualException.Message);
           
        }

        [Fact]
        public async Task Handle_Should_ThrowBookIsBorrowedException_WhenBookTransactionStatusIsBorrowed()
        {
            //Arrange
            _bookTransactionRepository.Setup(
                x => x.GetBookTransactionWhereStatusNotNone(
                      It.IsAny<Guid>())
                )
                .ReturnsAsync(() => _bookTransaction);

            _bookTransaction.Status = BookStatus.Borrowed;
            //Act

            async Task act() => await _handler.Handle(_command, default);
            BookIsBorrowedException ActualException = await Assert.ThrowsAsync<BookIsBorrowedException>(act);
            BookIsBorrowedException ExpectedException = new();

            //Assert
            _bookTransactionRepository.Verify(
               x => x.GetBookTransactionWhereStatusNotNone(
                    It.Is<Guid>(a => a == _bookTransaction.Id)
                   ),
               Times.Once);

            _bookTransactionRepository.Verify(
               x => x.ChangeBookTransactionStatusToBorrowed(
                   It.IsAny<BookTransaction>(),
                   It.IsAny<DateTime>()
                   ),
               Times.Never
               );

            _unitOfWorkMock.Verify(
             x => x.SaveChangesAsync(),
             Times.Never);

            Assert.Equal(BookStatus.Borrowed, _bookTransaction.Status);

            Assert.Equal(ExpectedException.Message, ActualException.Message);

        }
    }
}
