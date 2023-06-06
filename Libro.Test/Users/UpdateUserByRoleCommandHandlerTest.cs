using Libro.Application.Interfaces;
using Libro.Application.Users.Commands;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Exceptions.UserExceptions;
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
    public class UpdateUserByRoleCommandHandlerTest
    {
        private User _user;
        private readonly Role _role;
        private UserRole _userRole;
        private readonly UpdateUserByRoleCommand _command;
        private readonly UpdateUserByRoleCommandHandler _handler;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<ILogger<UpdateUserByRoleCommandHandler>> _loggerMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public UpdateUserByRoleCommandHandlerTest()
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
            _command = new(_user, _role.Name);
            _handler = new(
                _loggerMock.Object,
                _userRepositoryMock.Object,
                _unitOfWorkMock.Object
                );

        }

        [Fact]
        public async Task Handle_Should_ReturnTrue_WhenUserByRoleIsFoundAndEdited()
        {

            //Arrange
            _userRepositoryMock.Setup(
                x => x.GetUserWtithRolesAsync(
                    It.IsAny<Guid>()))
                .ReturnsAsync(() => _user);
           

            _userRepositoryMock.Setup(
               x => x.EmailIsUniqueForUpdateAsync(
                   It.IsAny<Guid>(),
                   It.IsAny<string>()))
               .ReturnsAsync(() => true);

            _userRepositoryMock.Setup(
              x => x.UserNameIsUniqueForUpdateAsync(
                  It.IsAny<Guid>(),
                  It.IsAny<string>()))
              .ReturnsAsync(() => true);

            _userRepositoryMock.Setup(
             x => x.PhoneNumberIsUniqueForUpdateAsync(
                 It.IsAny<Guid>(),
                 It.IsAny<string>()))
             .ReturnsAsync(() => true);

            _userRepositoryMock.Setup(
                x => x.UpdateUser(
                    It.IsAny<User>()));

            _unitOfWorkMock.Setup(
               x => x.SaveChangesAsync())
                .ReturnsAsync(1);


            //Act
            var result = await _handler.Handle(_command, default);

            //Assert
            _userRepositoryMock.Verify(
              x => x.GetUserWtithRolesAsync(It.Is<Guid>(x => x == _user.Id)),
              Times.Once);

            _userRepositoryMock.Verify(
              x => x.EmailIsUniqueForUpdateAsync(
                  It.Is<Guid>(u => u == _user.Id),
                  It.Is<string>(e => e == _user.Email)),
              Times.Once);

            _userRepositoryMock.Verify(
              x => x.UserNameIsUniqueForUpdateAsync(
                  It.Is<Guid>(u => u == _user.Id),
                  It.Is<string>(e => e == _user.UserName)),
              Times.Once);


            _userRepositoryMock.Verify(
             x => x.PhoneNumberIsUniqueForUpdateAsync(
                  It.Is<Guid>(u => u == _user.Id),
                  It.Is<string>(e => e == _user.PhoneNumber)),
              Times.Once);


            _userRepositoryMock.Verify(
                x => x.UpdateUser(
                    It.Is<User>(a => a.Id == _user.Id)),
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
              x => x.UpdateUser(
                  It.Is<User>(a => a.Id == _user.Id)),
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
              x => x.UpdateUser(
                  It.Is<User>(a => a.Id == _user.Id)),
              Times.Never);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
            Assert.Equal(ExpectedException.Message, ActualException.Message);

        }
        [Fact]
        public async Task Handle_Should_ThrowUserExistsException_WhenEmailIsNotUnique()
        {

            //Arrange
            _userRepositoryMock.Setup(
                x => x.GetUserWtithRolesAsync(
                    It.IsAny<Guid>()))
                .ReturnsAsync(() => _user);

            _userRepositoryMock.Setup(
                x => x.EmailIsUniqueForUpdateAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<string>()))
                .ReturnsAsync(false);

            //Act
            async Task act() => await _handler.Handle(_command, default);
            UserExistsException ActualException = await Assert.ThrowsAsync<UserExistsException>(act);
            UserExistsException ExpectedException = new($"{nameof(_user.Email)}");

            //Assert   

            _userRepositoryMock.Verify(
              x => x.GetUserWtithRolesAsync(It.Is<Guid>(x => x == _user.Id)),
              Times.Once);

            _userRepositoryMock.Verify(
          x => x.EmailIsUniqueForUpdateAsync(
              It.Is<Guid>(a => a == _user.Id),
              It.Is<string>(a => a == _user.Email)),
          Times.Once);

            _userRepositoryMock.Verify(
              x => x.UpdateUser(_user),
              Times.Never);

            _unitOfWorkMock.Verify(
               x => x.SaveChangesAsync(),
               Times.Never);

            Assert.Equal(ExpectedException.Message, ActualException.Message);


        }
        [Fact]
        public async Task Handle_Should_ThrowUserExistsException_WhenUserNameIsNotUnique()
        {

            //Arrange

            _userRepositoryMock.Setup(
           x => x.GetUserWtithRolesAsync(
               It.IsAny<Guid>()))
           .ReturnsAsync(() => _user);

            _userRepositoryMock.Setup(
               x => x.EmailIsUniqueForUpdateAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<string>()))
              .ReturnsAsync(true);

            _userRepositoryMock.Setup(
             x => x.UserNameIsUniqueForUpdateAsync(
                  It.IsAny<Guid>(),
                    It.IsAny<string>()))
             .ReturnsAsync(false);


            //Act
            async Task act() => await _handler.Handle(_command, default);
            UserExistsException Actual = await Assert.ThrowsAsync<UserExistsException>(act);
            UserExistsException ExpectedException = new($"{nameof(_user.UserName)}");

            //Assert   

            _userRepositoryMock.Verify(
          x => x.GetUserWtithRolesAsync(It.Is<Guid>(x => x == _user.Id)),
          Times.Once);

            _userRepositoryMock.Verify(
              x => x.EmailIsUniqueForUpdateAsync(
                  It.Is<Guid>(a => a == _user.Id),
                 It.Is<string>(a => a == _user.Email)),
              Times.Once);


            _userRepositoryMock.Verify(
             x => x.UserNameIsUniqueForUpdateAsync(
                 It.Is<Guid>(a => a == _user.Id),
                 It.Is<string>(a => a == _user.UserName)),
             Times.Once);

            _userRepositoryMock.Verify(
              x => x.UpdateUser(_user),
              Times.Never);
            _unitOfWorkMock.Verify(
               x => x.SaveChangesAsync(),
               Times.Never);
            Assert.Equal(ExpectedException.Message, Actual.Message);


        }

        [Fact]
        public async Task Handle_Should_ThrowUserExistsException_WhenPhoneNumberIsNotUnique()
        {

            //Arrange

            _userRepositoryMock.Setup(
         x => x.GetUserWtithRolesAsync(
             It.IsAny<Guid>()))
         .ReturnsAsync(() => _user);

            _userRepositoryMock.Setup(
              x => x.EmailIsUniqueForUpdateAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<string>()))
              .ReturnsAsync(true);

            _userRepositoryMock.Setup(
               x => x.UserNameIsUniqueForUpdateAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<string>()))
               .ReturnsAsync(true);

            _userRepositoryMock.Setup(
               x => x.PhoneNumberIsUniqueForUpdateAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<string>()))
               .ReturnsAsync(false);

            //Act
            async Task act() => await _handler.Handle(_command, default);
            UserExistsException Actual = await Assert.ThrowsAsync<UserExistsException>(act);
            UserExistsException ExpectedException = new($"{nameof(_user.PhoneNumber)}");

            //Assert   

            _userRepositoryMock.Verify(
       x => x.GetUserWtithRolesAsync(It.Is<Guid>(x => x == _user.Id)),
       Times.Once);

            _userRepositoryMock.Verify(
             x => x.EmailIsUniqueForUpdateAsync(
                 It.Is<Guid>(a => a == _user.Id),
                 It.Is<string>(a => a == _user.Email)),
             Times.Once);


            _userRepositoryMock.Verify(
             x => x.UserNameIsUniqueForUpdateAsync(
                  It.Is<Guid>(a => a == _user.Id),
                 It.Is<string>(a => a == _user.UserName)),
             Times.Once);

            _userRepositoryMock.Verify(
          x => x.PhoneNumberIsUniqueForUpdateAsync(
                 It.Is<Guid>(a => a == _user.Id),
                 It.Is<string>(a => a == _user.PhoneNumber)),
          Times.Once);

            _userRepositoryMock.Verify(
              x => x.UpdateUser(_user),
              Times.Never);

            _unitOfWorkMock.Verify(
               x => x.SaveChangesAsync(),
               Times.Never);
            Assert.Equal(ExpectedException.Message, Actual.Message);


        }
    }
}
