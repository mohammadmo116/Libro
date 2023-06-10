using Libro.Application.Books.Commands;
using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;

namespace Libro.Test.Books
{
    public class CreateBookCommandHandlerTest
    {
        private readonly Book _book;
        private readonly CreateBookCommand _command;
        private readonly CreateBookCommandHandler _handler;
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<ILogger<CreateBookCommandHandler>> _loggerMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        public CreateBookCommandHandlerTest()
        {
            _book = new()
            {
                Id = Guid.NewGuid(),
                Title = "title",
                IsAvailable = true,
                Genre = "genre"
            };
            _unitOfWorkMock = new();
            _bookRepositoryMock = new();
            _loggerMock = new();
            _command = new(_book);
            _handler = new(
                _loggerMock.Object,
                _bookRepositoryMock.Object,
                _unitOfWorkMock.Object
                );
        }
        [Fact]
        public async Task Handle_Should_ReturnBook_WhenBookIsCreated()
        {
            //Arrange
            _bookRepositoryMock.Setup(
              x => x.CreateBookAsync(
                  It.IsAny<Book>()));

            _unitOfWorkMock.Setup(
             x => x.SaveChangesAsync())
                .ReturnsAsync(1);
            //Act
            var result = await _handler.Handle(_command, default);

            //Assert

            _bookRepositoryMock.Verify(
              x => x.CreateBookAsync(It.Is<Book>(x => x.Id == _book.Id)),
              Times.Once);

            _unitOfWorkMock.Verify(
              x => x.SaveChangesAsync(),
              Times.Once);

            Assert.Equal(_book.Id, result.Id);
        }

    }
}
