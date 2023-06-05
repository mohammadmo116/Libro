using Libro.Application.ReadingLists.Commands;
using Libro.Domain.Entities;
using Libro.Infrastructure.Repositories;
using Libro.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libro.Application.ReadingLists.Queries;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Libro.Test.ReadingLists
{
    public class GetReadingListWithBooksQueryHandlerTest
    {
        private readonly ReadingList _readingList;
        private readonly GetReadingListWithBooksQuery _Query;
        private readonly GetReadingListWithBooksQueryHandler _handler;
        private readonly Mock<IReadingListRepository> _readingListRepositoryMock;
        private readonly Mock<ILogger<GetReadingListWithBooksQueryHandler>> _loggerMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        public GetReadingListWithBooksQueryHandlerTest()
        {
            _readingList = new()
            {
                Id = Guid.NewGuid(),
                Name = "readingList1"
            };
            
            _unitOfWorkMock = new();
            _readingListRepositoryMock = new();
            _loggerMock = new();
            _Query = new(_readingList.Id,1,1);
            _handler = new(
                _readingListRepositoryMock.Object,
                _loggerMock.Object,
                _unitOfWorkMock.Object
                );
        }
        [Fact]
        public async Task Handle_Should_ReturnReadingListWithBooks_WhenReadingListIsFound()
        {

            //Arrange
            _readingListRepositoryMock.Setup(
                x => x.GetReadingListWithBooksAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .ReturnsAsync(() => (_readingList,1));

            //Act
            var result = await _handler.Handle(_Query, default);

            //Assert
            _readingListRepositoryMock.Verify(
              x => x.GetReadingListWithBooksAsync(
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
                   It.IsAny<int>(),
                   It.IsAny<int>()))
               .ReturnsAsync(() => (null!,0));

            //Act
            var result = await _handler.Handle(_Query, default);

            //Assert

            _readingListRepositoryMock.Verify(
            x => x.GetReadingListWithBooksAsync(
                It.Is<Guid>(x => x == _readingList.Id),
                It.IsAny<int>(),
               It.IsAny<int>()
                ),
            Times.Once);
            Assert.Null(result.Item1);

        }
    }
}
