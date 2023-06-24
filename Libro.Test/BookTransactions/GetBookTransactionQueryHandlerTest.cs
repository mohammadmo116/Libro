using Libro.Application.BookTransactions.Queries;
using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Libro.Test.BookTransactions
{
    public class GetBookTransactionQueryHandlerTest
    {

        private readonly BookTransaction _bookTransaction;
        private readonly User _user;
        private readonly GetBookTransactionQuery _query;
        private readonly GetBookTransactionQueryHandler _handler;
        private readonly Mock<IBookTransactionRepository> _bookTransactionRepository;
        private readonly Mock<ILogger<GetBookTransactionQueryHandler>> _loggerMock;

        public GetBookTransactionQueryHandlerTest()
        {

            _bookTransactionRepository = new();
            _loggerMock = new();

            _user = new()
            {
                Id = Guid.NewGuid(),
                Email = "test@test.com",
                UserName = "userName"

            };
            _bookTransaction = new()
            {
                Id = Guid.NewGuid(),
                BookId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
            };

            _query = new(_user.Id, _bookTransaction.Id);

            _handler = new(
               _loggerMock.Object,
               _bookTransactionRepository.Object
               );
        }
        [Fact]
        public async Task Handle_Should_ReturnBookTransaction_WhenTransactionIsFound()
        {
            //Arrange
            _bookTransactionRepository.Setup(
                x => x.GetUserBookTransactionAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>())
                )
                .ReturnsAsync(() => _bookTransaction);


            //Act
            var result = await _handler.Handle(_query, default);


            //Assert

            Assert.Equal(result.Id, _bookTransaction.Id);

            _bookTransactionRepository.Verify(
               x => x.GetUserBookTransactionAsync(
                   It.Is<Guid>(a => a == _user.Id),
                   It.Is<Guid>(a => a == _bookTransaction.Id)
                   ),
               Times.Once);



        }
        [Fact]
        public async Task Handle_Should_ThrowCustomNotFoundException_WhenTransactionIsNotFound()
        {
            //Arrange
            _bookTransactionRepository.Setup(
                x => x.GetUserBookTransactionAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>())
                )
                .ReturnsAsync(() => null!);


            //Act            
            async Task act() => await _handler.Handle(_query, default);
            CustomNotFoundException ActualException = await Assert.ThrowsAsync<CustomNotFoundException>(act);
            CustomNotFoundException ExpectedException = new("BookTransaction");

            //Assert

            Assert.Equal(ExpectedException.Message, ActualException.Message);

            _bookTransactionRepository.Verify(
               x => x.GetUserBookTransactionAsync(
                   It.Is<Guid>(a => a == _user.Id),
                   It.Is<Guid>(a => a == _bookTransaction.Id)
                   ),
               Times.Once);


        }
    }
}
