using Libro.Application.Interfaces;
using Libro.Application.Users.Commands;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;

namespace Libro.Test.Users
{
    public class RemoveUserCommandHandlerTest
    {
        private readonly User _user;
        private readonly RemoveUserCommand _command;
        private readonly RemoveUserCommandHandler _handler;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILogger<RemoveUserCommandHandler>> _loggerMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public RemoveUserCommandHandlerTest()
        {
            _userRepositoryMock = new();
            _loggerMock = new();
            _unitOfWorkMock = new();
            _user = new();
            _command = new(_user.Id);
            _handler = new(
                _loggerMock.Object,
                _userRepositoryMock.Object,
                _unitOfWorkMock.Object
                );

        }
        [Fact]
        public async Task Handle_Should_ReturnTrue_WhenUserIsFoundAndDeleted()
        {

            //Arrange
            _userRepositoryMock.Setup(
                x => x.GetUserAsync(
                    It.IsAny<Guid>()))
                .ReturnsAsync(() => _user);

            _userRepositoryMock.Setup(
              x => x.RemoveUser(
                  It.IsAny<User>()));

            _unitOfWorkMock.Setup(
               x => x.SaveChangesAsync())
                .ReturnsAsync(2);


            //Act
            var result = await _handler.Handle(_command, default);

            //Assert
            _userRepositoryMock.Verify(
              x => x.GetUserAsync(It.Is<Guid>(x => x == _user.Id)),
              Times.Once);

            _userRepositoryMock.Verify(
             x => x.RemoveUser(
                 It.Is<User>(b => b.Id == _user.Id)),
             Times.Once);


            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);

            Assert.True(result);

        }

        [Fact]
        public async Task Handle_Should_ThrowCustomNotFoundException_WhenUserIsNotFound()
        {

            //Arrange
            _userRepositoryMock.Setup(
                x => x.GetUserAsync(
                    It.IsAny<Guid>()))
                .ReturnsAsync(() => null!);

            //Act
            async Task act() => await _handler.Handle(_command, default);
            CustomNotFoundException ActualException = await Assert.ThrowsAsync<CustomNotFoundException>(act);
            CustomNotFoundException ExpectedException = new($"User");

            //Assert
            _userRepositoryMock.Verify(
                x => x.GetUserAsync(It.Is<Guid>(x => x == _user.Id)),
                Times.Once);

            _userRepositoryMock.Verify(
           x => x.RemoveUser(
               It.Is<User>(b => b.Id == _user.Id)),
           Times.Never);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);

            Assert.Equal(ExpectedException.Message, ActualException.Message);

        }

    }
}
