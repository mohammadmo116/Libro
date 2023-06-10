using Libro.Application.Interfaces;
using Libro.Application.Users.Queries;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Assert = Xunit.Assert;

namespace Libro.Test.Users
{
    public sealed class GetPatronRecommendedBooksQueryHandlerTest
    {

        private readonly Role _admin;
        private readonly Role _patron;
        private readonly List<Book> _books;
        private readonly User _user;
        private readonly GetPatronRecommendedBooksQuery _query;
        private readonly GetPatronRecommendedBooksQueryHandler _handler;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILogger<GetPatronRecommendedBooksQueryHandler>> _loggerMock;
        public GetPatronRecommendedBooksQueryHandlerTest()
        {
            var AuthorsList = new List<Author> {
                    new Author(){
                        Id= Guid.NewGuid(),
                        Name= "author1",
                    },
                      new Author(){
                        Id= Guid.NewGuid(),
                        Name= "author2",
                    },
            };

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

            _books = new()
            {new(){
                   Id = Guid.NewGuid(),
                   Genre="rst",
                   IsAvailable=true,
                   Title="tset",
                   Authors=AuthorsList


            },
            new(){
                      Id = Guid.NewGuid(),
                   Genre="rst",
                   IsAvailable=true,
                   Title="tset",
                   Authors=AuthorsList

        },
            new(){
                   Id = Guid.NewGuid(),
                   Genre="rst",
                   IsAvailable=true,
                   Title="tset",
                    Authors=AuthorsList

            },
            };
            _userRepositoryMock = new();
            _loggerMock = new();
            _query = new(_user.Id, 1, 1);
            _handler = new(
                _loggerMock.Object,
                _userRepositoryMock.Object
                );
        }

        [Fact]
        public async Task Handle_Should_ReturnRecommendedBooks_WhenPatronUserIsFound()

        {

            //Arrange
            _userRepositoryMock.Setup(
                x => x.GetUserWtithRolesAsync(
                    It.IsAny<Guid>()))
                .ReturnsAsync(() => _user);

            _user.Roles.Add(_patron);

            _userRepositoryMock.Setup(
                x => x.GetRecommendedBooksAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .ReturnsAsync(() => (_books, 1));


            //Act

            var result = await _handler.Handle(_query, default);

            //Assert
            _userRepositoryMock.Verify(
              x => x.GetUserWtithRolesAsync(It.Is<Guid>(x => x == _user.Id)),
              Times.Once);

            _userRepositoryMock.Verify(
                x => x.GetRecommendedBooksAsync(
                    It.Is<Guid>(u => u == _user.Id),
                    It.Is<int>(p => p == 1),
                    It.Is<int>(c => c == 1)),
                Times.Once);

            CollectionAssert
                .AreEqual(_books, result.Item1);


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

            async Task act() => await _handler.Handle(_query, default);
            CustomNotFoundException ActualException = await Assert.ThrowsAsync<CustomNotFoundException>(act);
            CustomNotFoundException ExpectedException = new("User");

            //Assert
            _userRepositoryMock.Verify(
              x => x.GetUserWtithRolesAsync(It.Is<Guid>(x => x == _user.Id)),
              Times.Once);
            _userRepositoryMock.Verify(
              x => x.GetRecommendedBooksAsync(
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

            _user.Roles.Remove(_patron);
            _user.Roles.Add(_admin);

            //Act
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
