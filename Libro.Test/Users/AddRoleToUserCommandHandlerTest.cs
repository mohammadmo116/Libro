using Libro.Application.Interfaces;
using Libro.Application.Users.Commands;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Test.Users
{
    public class AddRoleToUserCommandHandlerTest
    {
        private readonly User _user;
        private readonly Role _role;

        private readonly UserRole _userRole;
        private readonly AddRoleToUserCommand _command;
        private readonly AddRoleToUserCommandHandler _handler;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILogger<AddRoleToUserCommandHandler>> _loggerMock;

        public AddRoleToUserCommandHandlerTest()
        {
            _user = new()
            {
                Id = Guid.NewGuid(),
                Email = "ads@gmail.com",
                Password = "password",
                PhoneNumber = "12345",
                UserName = "Test"
            };

            Role PreviousRole = new()
            {
                Id = Guid.NewGuid(),
                Name = "Test"
            };

            _role = new()
            {
                Id = Guid.NewGuid(),
                Name = "Admin",
            };

            _user.Roles.Add(PreviousRole);
            _user.Roles.Add(_role);

            _userRepositoryMock = new();
            _loggerMock = new();
            _userRole = new()
            {
                RoleId = _role.Id,
                UserId = _user.Id
            };
            _command = new AddRoleToUserCommand(_userRole);
            _handler = new AddRoleToUserCommandHandler(
                _loggerMock.Object,
                _userRepositoryMock.Object
                );
        }

        [Fact]
        public async Task Handle_Should_ReturnTheUserRoles_WhenSuccess()
        {

            //Arrange
            _userRepositoryMock.Setup(
           x => x.RoleOrUserNotFoundAsync(
               It.IsAny<UserRole>()))
           .ReturnsAsync(false);

            _userRepositoryMock.Setup(
         x => x.UserHasTheAssignedRoleAsync(
             It.IsAny<UserRole>()))
         .ReturnsAsync(false);

            _userRepositoryMock.Setup(
                x => x.AssignRoleToUserAsync(
                    It.IsAny<UserRole>()))
                .ReturnsAsync(_user.Roles.Select(a => a.Name).ToList());

            //Act
            var result = await _handler.Handle(_command, default);

            //Assert
            _userRepositoryMock.Verify(
             x => x.RoleOrUserNotFoundAsync(_userRole),
             Times.Once);
            _userRepositoryMock.Verify(
             x => x.UserHasTheAssignedRoleAsync(_userRole),
             Times.Once);

            _userRepositoryMock.Verify(
             x => x.AssignRoleToUserAsync(It.Is<UserRole>(u => u.RoleId == _role.Id && u.UserId == _user.Id)),
             Times.Once);
            Assert.Contains(_role.Name, result);
        }
        [Fact]
        public async Task Handle_Should_ThrowUserOrRoleNotFoundException_WhenUserOrRoleNotFound()
        {
            //Arrange
            _userRepositoryMock.Setup(
           x => x.RoleOrUserNotFoundAsync(
               It.IsAny<UserRole>()))
           .ReturnsAsync(true);


            //Act
            async Task act() => await _handler.Handle(_command, default);
            UserOrRoleNotFoundException Actual = await Assert.ThrowsAsync<UserOrRoleNotFoundException>(act);
            UserOrRoleNotFoundException ExpectedException = new();

            //Assert   
            _userRepositoryMock.Verify(
             x => x.RoleOrUserNotFoundAsync(_userRole),
             Times.Once);
            _userRepositoryMock.Verify(
             x => x.UserHasTheAssignedRoleAsync(_userRole),
             Times.Never);
            _userRepositoryMock.Verify(
              x => x.AssignRoleToUserAsync(_userRole),
              Times.Never);
            Assert.Equal(ExpectedException.Message, Actual.Message);

        }
        [Fact]
        public async Task Handle_Should_ThrowUserHasTheAssignedRoleException_WhenUserHasTheAssignedRole()
        {
            //Arrange
            _userRepositoryMock.Setup(
             x => x.RoleOrUserNotFoundAsync(
                 It.IsAny<UserRole>()))
             .ReturnsAsync(false);

            _userRepositoryMock.Setup(
           x => x.UserHasTheAssignedRoleAsync(
               It.IsAny<UserRole>()))
           .ReturnsAsync(true);

            //Act
            async Task act() => await _handler.Handle(_command, default);
            UserHasTheAssignedRoleException Actual = await Assert.ThrowsAsync<UserHasTheAssignedRoleException>(act);
            UserHasTheAssignedRoleException ExpectedException = new();

            //Assert   
            _userRepositoryMock.Verify(
            x => x.RoleOrUserNotFoundAsync(_userRole),
            Times.Once);
            _userRepositoryMock.Verify(
             x => x.UserHasTheAssignedRoleAsync(_userRole),
             Times.Once);
            _userRepositoryMock.Verify(
              x => x.AssignRoleToUserAsync(_userRole),
              Times.Never);
            Assert.Equal(ExpectedException.Message, Actual.Message);

        }
    }
}
