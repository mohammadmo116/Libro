using Libro.Application.Books.Commands;
using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;

namespace Libro.Test.Books
{
    public class UpdateBookCommandHandlerTest
    {
        private readonly Book _book;
        private readonly UpdateBookCommand _command;
        private readonly UpdateBookCommandHandler _handler;
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<ILogger<UpdateBookCommandHandler>> _loggerMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        public UpdateBookCommandHandlerTest()
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
        public async Task Handle_Should_ReturnTrue_WhenBookIsFoundAndEdited()
        {

            //Arrange
            _bookRepositoryMock.Setup(
                x => x.GetBookAsync(
                    It.IsAny<Guid>()))
                .ReturnsAsync(() => _book);

            _bookRepositoryMock.Setup(
              x => x.UpdateBook(
                  It.IsAny<Book>()));

            _unitOfWorkMock.Setup(
               x => x.SaveChangesAsync())
                .ReturnsAsync(1);


            //Act
            var result = await _handler.Handle(_command, default);

            //Assert
            _bookRepositoryMock.Verify(
              x => x.GetBookAsync(It.Is<Guid>(x => x == _book.Id)),
              Times.Once);

            _bookRepositoryMock.Verify(
             x => x.UpdateBook(
                 It.Is<Book>(b => b.Id == _book.Id)),
             Times.Once);


            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);

            Assert.True(result);

        }
        [Fact]
        public async Task Handle_Should_ThrowCustomNotFoundException_WhenBookIsNotFound()
        {

            //Arrange
            _bookRepositoryMock.Setup(
                x => x.GetBookAsync(
                    It.IsAny<Guid>()))
                .ReturnsAsync(() => null!);

            //Act
            async Task act() => await _handler.Handle(_command, default);
            CustomNotFoundException ActualException = await Assert.ThrowsAsync<CustomNotFoundException>(act);
            CustomNotFoundException ExpectedException = new("Book");

            //Assert
            _bookRepositoryMock.Verify(
                x => x.GetBookAsync(It.Is<Guid>(x => x == _book.Id)),
                Times.Once);

            _bookRepositoryMock.Verify(
           x => x.UpdateBook(
               It.Is<Book>(b => b.Id == _book.Id)),
           Times.Never);


            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
            Assert.Equal(ExpectedException.Message, ActualException.Message);

        }

    }
}
