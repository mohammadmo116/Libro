using Libro.Application.ReadingLists.Queries;
using Libro.Domain.Entities;
using Libro.Infrastructure.Repositories;
using Moq;
using NUnit.Framework;
using Assert = Xunit.Assert;

namespace Libro.Test.ReadingLists
{
    public class GetUserReadingListsQueryHandlerTest
    {
        private readonly User _user;
        private readonly List<ReadingList> _readingLists;
        private readonly GetUserReadingListsQuery _Query;
        private readonly GetUserReadingListsQueryHandler _handler;
        private readonly Mock<IReadingListRepository> _readingListRepositoryMock;

        public GetUserReadingListsQueryHandlerTest()
        {
            _readingLists = new()
            {
                new(){
                Id = Guid.NewGuid(),
                Name = "readingList1"
                },
                  new(){
                Id = Guid.NewGuid(),
                Name = "readingList2"
                },
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
            _Query = new(_user.Id, 1, 1);
            _handler = new(
                _readingListRepositoryMock.Object
                );
        }
        [Fact]
        public async Task Handle_Should_ReturnUserReadingLists_WhenSuccess()
        {

            //Arrange
            _readingListRepositoryMock.Setup(
                x => x.GetReadingListsByUserAsync(
                     It.IsAny<Guid>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .ReturnsAsync(() => (_readingLists, 1));

            //Act
            var result = await _handler.Handle(_Query, default);

            //Assert
            CollectionAssert.AreEqual(_readingLists, result.Item1);
            Assert.Equal(1, result.Item2);


            _readingListRepositoryMock.Verify(
              x => x.GetReadingListsByUserAsync(
                   It.Is<Guid>(x => x == _user.Id),
                  It.Is<int>(x => x == 1),
                 It.Is<int>(x => x == 1)
                  ),
              Times.Once);

        }
    }
}
