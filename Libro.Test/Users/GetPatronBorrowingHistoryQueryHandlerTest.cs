using Libro.Application.Interfaces;
using Libro.Application.Users.Queries;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Assert = Xunit.Assert;
using TheoryAttribute = Xunit.TheoryAttribute;

namespace Libro.Test.Users
{
    public class GetPatronBorrowingHistoryQueryHandlerTest
    {
        private readonly Role _admin;
        private readonly Role _patron;
        private readonly List<BookTransaction> _bookTransactionsList;
        private readonly User _user;
        private readonly GetPatronBorrowingHistoryQueryHandler _handler;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILogger<GetPatronBorrowingHistoryQueryHandler>> _loggerMock;
        public GetPatronBorrowingHistoryQueryHandlerTest()
        {
            _admin = new()
            {
                Id = Guid.NewGuid(),
                Name = "admin",

            };
            _patron = new()
            {
                Id = Guid.NewGuid(),
                Name = "patron",

            };
            _user = new()
            {
                Id = Guid.NewGuid(),
                Email = "sad@gmail.com",
                Password = "Password",
                PhoneNumber = "PhoneNumber",
                UserName = "UserName"
            };

            _bookTransactionsList = new()
            {new(){
                   Id = Guid.NewGuid(),
                   BookId = Guid.NewGuid(),
                   UserId = Guid.NewGuid(),

            },
            new(){
                   Id = Guid.NewGuid(),
                   BookId = Guid.NewGuid(),
                   UserId = Guid.NewGuid(),
            },
            new(){
                   Id = Guid.NewGuid(),
                   BookId = Guid.NewGuid(),
                   UserId = Guid.NewGuid(),
            },
            };
            _userRepositoryMock = new();
            _loggerMock = new();

            _handler = new(
                _loggerMock.Object,
                _userRepositoryMock.Object
                );
        }


        [Theory]
        [InlineData(0, 1)]
        [InlineData(2, 1)]
        [InlineData(5, 5)]
        public async Task Handle_Should_ReturnBorrowingHistory_WhenPatronUserIsFound(
            int PageNumber,
            int Count
            )
        {

            //Arrange
            _userRepositoryMock.Setup(
                x => x.GetUserWtithRolesAsync(
                    It.IsAny<Guid>()))
                .ReturnsAsync(() => _user);

            _user.Roles.Add(_patron);

            _userRepositoryMock.Setup(
                x => x.GetBorrowingHistoryAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .ReturnsAsync(() => (
                _bookTransactionsList
                    .Skip(PageNumber * Count)
                    .Take(Count)
                    .ToList(),
                    1)
                    );


            //Act
            var _query = new GetPatronBorrowingHistoryQuery(_user.Id, PageNumber, Count);
            var result = await _handler.Handle(_query, default);

            //Assert
            _userRepositoryMock.Verify(
              x => x.GetUserWtithRolesAsync(It.Is<Guid>(x => x == _user.Id)),
              Times.Once);

            _userRepositoryMock.Verify(
                x => x.GetBorrowingHistoryAsync(
                    It.Is<Guid>(u => u == _user.Id),
                    It.Is<int>(p => p == PageNumber),
                    It.Is<int>(c => c == Count)),
                Times.Once);

            CollectionAssert
                .AreEqual(_bookTransactionsList.Skip(PageNumber * Count).Take(Count).ToList(), result.Item1);


        }
        [Fact]
        public async Task Handle_Should_ThrowCustomNotFoundException_WhenUserIsNotFound()
        {

            //Arrange
            _userRepositoryMock.Setup(
                x => x.GetUserWtithRolesAsync(
                    It.IsAny<Guid>()))
                .ReturnsAsync(() => null!);

            //Act
            var _query = new GetPatronBorrowingHistoryQuery(_user.Id, 1, 1);
            async Task act() => await _handler.Handle(_query, default);
            CustomNotFoundException ActualException = await Assert.ThrowsAsync<CustomNotFoundException>(act);
            CustomNotFoundException ExpectedException = new("User");

            //Assert
            _userRepositoryMock.Verify(
              x => x.GetUserWtithRolesAsync(It.Is<Guid>(x => x == _user.Id)),
              Times.Once);
            _userRepositoryMock.Verify(
              x => x.GetBorrowingHistoryAsync(
                  It.IsAny<Guid>(),
                  It.IsAny<int>(),
                  It.IsAny<int>()),
              Times.Never);
            Assert.Equal(ExpectedException.Message, ActualException.Message);

        }

        [Fact]
        public async Task Handle_Should_ThrowCustomNotFoundException_WhenUserIsNotPatron()
        {

            //Arrange
            _userRepositoryMock.Setup(
                x => x.GetUserWtithRolesAsync(
                    It.IsAny<Guid>()))
                .ReturnsAsync(() => _user);

            _user.Roles.Add(_admin);

            //Act
            var _query = new GetPatronBorrowingHistoryQuery(_user.Id, 1, 1);
            async Task act() => await _handler.Handle(_query, default);
            CustomNotFoundException ActualException = await Assert.ThrowsAsync<CustomNotFoundException>(act);
            CustomNotFoundException ExpectedException = new("User");

            //Assert
            _userRepositoryMock.Verify(
              x => x.GetUserWtithRolesAsync(It.Is<Guid>(x => x == _user.Id)),
              Times.Once);
            _userRepositoryMock.Verify(
              x => x.GetBorrowingHistoryAsync(
                  It.IsAny<Guid>(),
                  It.IsAny<int>(),
                  It.IsAny<int>()),
              Times.Never);
            Assert.Equal(ExpectedException.Message, ActualException.Message);

        }
    }
}
