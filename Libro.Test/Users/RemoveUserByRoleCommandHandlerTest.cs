using Libro.Application.Interfaces;
using Libro.Application.Users.Commands;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Test.Users
{
    public class RemoveUserByRoleCommandHandlerTest
    {
        private User _user;
        private readonly Role _role;
        private UserRole _userRole;
        private readonly RemoveUserByRoleCommand _command;
        private readonly RemoveUserByRoleCommandHandler _handler;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<ILogger<RemoveUserByRoleCommandHandler>> _loggerMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public RemoveUserByRoleCommandHandlerTest()
        {
            _userRepositoryMock = new();
            _roleRepositoryMock = new();
            _loggerMock = new();
            _unitOfWorkMock = new();
            _user = new();
            _role = new()
            {
                Id = Guid.NewGuid(),
                Name = "librarian"
            };
            _user.Roles.Add(_role);
            _command = new(_user.Id, _role.Name);
            _handler = new(
                _loggerMock.Object,
                _userRepositoryMock.Object,
                _unitOfWorkMock.Object
                );

        }
        [Fact]
        public async Task Handle_Should_ReturnTrue_WhenUserByRoleIsFoundAndDeleted()
        {

            //Arrange
            _userRepositoryMock.Setup(
                x => x.GetUserWtithRolesAsync(
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
              x => x.GetUserWtithRolesAsync(It.Is<Guid>(x => x == _user.Id)),
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
        public async Task Handle_Should_ThrowCustomNotFoundException_WhenUserByRoleIsNotFound()
        {

            //Arrange
            _userRepositoryMock.Setup(
                x => x.GetUserWtithRolesAsync(
                    It.IsAny<Guid>()))
                .ReturnsAsync(() => null!);

            //Act
            async Task act() => await _handler.Handle(_command, default);
            CustomNotFoundException ActualException = await Assert.ThrowsAsync<CustomNotFoundException>(act);
            CustomNotFoundException ExpectedException = new($"User - Role {_role.Name}");

            //Assert
            _userRepositoryMock.Verify(
                x => x.GetUserWtithRolesAsync(It.Is<Guid>(x => x == _user.Id)),
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
        [Fact]
        public async Task Handle_Should_ThrowCustomNotFoundException_WhenUserDoesNotHasTheRole()
        {

            //Arrange
            _userRepositoryMock.Setup(
                x => x.GetUserWtithRolesAsync(
                    It.IsAny<Guid>()))
                .ReturnsAsync(() => _user);

            _user.Roles.Remove(_role);

            //Act
            async Task act() => await _handler.Handle(_command, default);
            CustomNotFoundException ActualException = await Assert.ThrowsAsync<CustomNotFoundException>(act);
            CustomNotFoundException ExpectedException = new($"User - Role {_role.Name}");

            //Assert
            _userRepositoryMock.Verify(
                x => x.GetUserWtithRolesAsync(It.Is<Guid>(x => x == _user.Id)),
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
