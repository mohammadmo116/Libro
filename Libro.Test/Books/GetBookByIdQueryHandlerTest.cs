using Libro.Application.Books.Queries;
using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Libro.Test.Books
{
    public class GetBookByIdQueryHandlerTest
    {
        private readonly Book _book;
        private readonly GetBookByIdQuery _Query;
        private readonly GetBookByIdQueryHandler _handler;
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<ILogger<GetBookByIdQueryHandler>> _loggerMock;
        public GetBookByIdQueryHandlerTest()
        {
            _book = new()
            {
                Id = Guid.NewGuid(),
                Title = "title",
                IsAvailable = true,
                Genre = "genre"
            };

            _bookRepositoryMock = new();
            _loggerMock = new();
            _Query = new(_book.Id);
            _handler = new(
                _loggerMock.Object,
                _bookRepositoryMock.Object
                );
        }
        [Fact]
        public async Task Handle_Should_ReturnBook_WhenBookIsFound()
        {

            //Arrange
            _bookRepositoryMock.Setup(
                x => x.GetBookAsync(
                    It.IsAny<Guid>()))
                .ReturnsAsync(() => _book);

            //Act
            var result = await _handler.Handle(_Query, default);

            //Assert
            _bookRepositoryMock.Verify(
              x => x.GetBookAsync(It.Is<Guid>(x => x == _book.Id)),
              Times.Once);
            Assert.Equal(_book.Id, result.Id);

        }
        [Fact]
        public async Task Handle_Should_ReturnNull_WhenBookNotFound()
        {

            //Arrange
            _bookRepositoryMock.Setup(
                x => x.GetBookAsync(
                    It.IsAny<Guid>()))
                .ReturnsAsync(() => null!);

            //Act
            var result = await _handler.Handle(_Query, default);

            //Assert

            _bookRepositoryMock.Verify(
            x => x.GetBookAsync(It.Is<Guid>(x => x == _book.Id)),
            Times.Once);
            Assert.Null(result);

        }
    }
}
