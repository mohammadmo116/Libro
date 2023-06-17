using Libro.Application.ReadingLists.Queries;
using Libro.Domain.Entities;
using Libro.Infrastructure;
using Libro.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Libro.Test.ReadingLists
{
    public class GetUserReadingListWithBooksQueryHandlerTest
    {
        private readonly User _user;
        private readonly ReadingList _readingList;
        private readonly GetUserReadingListWithBooksQuery _Query;
        private readonly GetUserReadingListWithBooksQueryHandler _handler;
        private readonly Mock<IReadingListRepository> _readingListRepositoryMock;

        public GetUserReadingListWithBooksQueryHandlerTest()
        {
            _readingList = new()
            {
                Id = Guid.NewGuid(),
                Name = "readingList1"
            };
            _user = new()
            {
                Id = Guid.NewGuid(),
                Email = "ads@gmail.com",
                Password = "password",
                PhoneNumber = "12345",
                UserName = "Test"
            };
            _readingListRepositoryMock = new();
            _Query = new(_user.Id, _readingList.Id, 1, 1);
            _handler = new(
                _readingListRepositoryMock.Object
                );
        }
        [Fact]
        public async Task Handle_Should_ReturnReadingListWithBooks_WhenReadingListIsFound()
        {

            //Arrange
            _readingListRepositoryMock.Setup(
                x => x.GetReadingListWithBooksAsync(
                     It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .ReturnsAsync(() => (_readingList, 1));

            //Act
            var result = await _handler.Handle(_Query, default);

            //Assert
            _readingListRepositoryMock.Verify(
              x => x.GetReadingListWithBooksAsync(
                   It.Is<Guid>(x => x == _user.Id),
                  It.Is<Guid>(x => x == _readingList.Id),
                  It.IsAny<int>(),
                 It.IsAny<int>()
                  ),
              Times.Once);
            Assert.Equal(_readingList.Id, result.Item1.Id);

        }
        [Fact]
        public async Task Handle_Should_ReturnNull_WhenBookNotFound()
        {

            //Arrange
            _readingListRepositoryMock.Setup(
                 x => x.GetReadingListWithBooksAsync(
                      It.IsAny<Guid>(),
                     It.IsAny<Guid>(),
                     It.IsAny<int>(),
                     It.IsAny<int>()))
                .ReturnsAsync(() => (null!, 0));

            //Act
            var result = await _handler.Handle(_Query, default);

            //Assert

            _readingListRepositoryMock.Verify(
            x => x.GetReadingListWithBooksAsync(
                 It.Is<Guid>(x => x == _user.Id),
                It.Is<Guid>(x => x == _readingList.Id),
                It.IsAny<int>(),
               It.IsAny<int>()
                ),
            Times.Once);
            Assert.Null(result.Item1);

        }
    }
}
