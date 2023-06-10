using Libro.Application.Authors.Queries;
using Libro.Application.Repositories;
using Libro.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Libro.Test.Authors
{
    public class GetAuthorQueryHandlerTest
    {
        private readonly Author _author;
        private readonly GetAuthorQuery _query;
        private readonly GetAuthorQueryHandler _handler;
        private readonly Mock<IAuthorRepository> _authorRepositoryMock;
        private readonly Mock<ILogger<GetAuthorQueryHandler>> _loggerMock;
        public GetAuthorQueryHandlerTest()
        {
            _author = new()
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                DateOfBirth = DateTime.UtcNow
            };
            _authorRepositoryMock = new();
            _loggerMock = new();
            _query = new(_author.Id);
            _handler = new(
                _loggerMock.Object,
                _authorRepositoryMock.Object
                );
        }
        [Fact]
        public async Task Handle_Should_ReturnAuthor_WhenAuthorIsFound()
        {

            //Arrange
            _authorRepositoryMock.Setup(
                x => x.GetAuthorAsync(
                    It.IsAny<Guid>()))
                .ReturnsAsync(() => _author);

            //Act
            var result = await _handler.Handle(_query, default);

            //Assert
            _authorRepositoryMock.Verify(
              x => x.GetAuthorAsync(It.Is<Guid>(x => x == _author.Id)),
              Times.Once);

            Assert.Equal(_author.Id, result.Id);

        }
        [Fact]
        public async Task Handle_Should_ReturnNull_WhenBookNotFound()
        {

            //Arrange
            _authorRepositoryMock.Setup(
                x => x.GetAuthorAsync(
                    It.IsAny<Guid>()))
                .ReturnsAsync(() => null!);

            //Act
            var result = await _handler.Handle(_query, default);

            //Assert

            _authorRepositoryMock.Verify(
            x => x.GetAuthorAsync(It.Is<Guid>(x => x == _author.Id)),
            Times.Once);
            Assert.Null(result);

        }
    }
}
