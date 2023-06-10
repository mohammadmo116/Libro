using Libro.Application.Authors.Commands;
using Libro.Application.Repositories;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;

namespace Libro.Test.Authors
{
    public class RemoveAuthorCommandHandlerTest
    {

        private readonly Author _author;
        private readonly RemoveAuthorCommand _command;
        private readonly RemoveAuthorCommandHandler _handler;
        private readonly Mock<IAuthorRepository> _authorRepositoryMock;
        private readonly Mock<ILogger<RemoveAuthorCommandHandler>> _loggerMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        public RemoveAuthorCommandHandlerTest()
        {
            _author = new()
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                DateOfBirth = DateTime.UtcNow
            };
            _unitOfWorkMock = new();
            _authorRepositoryMock = new();
            _loggerMock = new();
            _command = new(_author.Id);
            _handler = new(
                _loggerMock.Object,
                _authorRepositoryMock.Object,
                _unitOfWorkMock.Object
                );
        }
        [Fact]
        public async Task Handle_Should_ReturnTrue_WhenAuthorIsFoundAndDeleted()
        {

            //Arrange
            _authorRepositoryMock.Setup(
                x => x.GetAuthorAsync(
                    It.IsAny<Guid>()))
                .ReturnsAsync(() => _author);

            _authorRepositoryMock.Setup(
              x => x.RemoveAuthor(
                  It.IsAny<Author>()));

            _unitOfWorkMock.Setup(
               x => x.SaveChangesAsync())
                .ReturnsAsync(1);


            //Act
            var result = await _handler.Handle(_command, default);

            //Assert
            _authorRepositoryMock.Verify(
              x => x.GetAuthorAsync(It.Is<Guid>(x => x == _author.Id)),
              Times.Once);

            _authorRepositoryMock.Verify(
             x => x.RemoveAuthor(
                 It.Is<Author>(b => b.Id == _author.Id)),
             Times.Once);


            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);

            Assert.True(result);

        }

        [Fact]
        public async Task Handle_Should_ThrowCustomNotFoundException_WhenAuthorIsNotFound()
        {

            //Arrange
            _authorRepositoryMock.Setup(
                x => x.GetAuthorAsync(
                    It.IsAny<Guid>()))
                .ReturnsAsync(() => null!);

            //Act
            async Task act() => await _handler.Handle(_command, default);
            CustomNotFoundException ActualException = await Assert.ThrowsAsync<CustomNotFoundException>(act);
            CustomNotFoundException ExpectedException = new("Author");

            //Assert
            _authorRepositoryMock.Verify(
                x => x.GetAuthorAsync(It.Is<Guid>(x => x == _author.Id)),
                Times.Once);

            _authorRepositoryMock.Verify(
           x => x.RemoveAuthor(
               It.Is<Author>(b => b.Id == _author.Id)),
           Times.Never);


            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
            Assert.Equal(ExpectedException.Message, ActualException.Message);

        }
    }
}
