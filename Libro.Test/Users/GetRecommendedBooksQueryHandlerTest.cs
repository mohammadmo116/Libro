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
    public sealed class GetRecommendedBooksQueryHandlerTest
    {
        private readonly List<Book> _books;
        private readonly User _user;
        private readonly GetRecommendedBooksQuery _query;
        private readonly GetRecommendedBooksQueryHandler _handler;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILogger<GetRecommendedBooksQueryHandler>> _loggerMock;
        public GetRecommendedBooksQueryHandlerTest()
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
        public async Task Handle_Should_ReturnRecommendedBooks_WhenUserIsFound()

        {

            //Arrange
         
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
                x => x.GetRecommendedBooksAsync(
                    It.Is<Guid>(u => u == _user.Id),
                    It.Is<int>(p => p == 1),
                    It.Is<int>(c => c == 1)),
                Times.Once);

            CollectionAssert
                .AreEqual(_books, result.Item1);


        }
        

    }
}
