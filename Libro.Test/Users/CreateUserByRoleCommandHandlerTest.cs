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
    public class CreateUserByRoleCommandHandlerTest
    {

        private User _user;
        private readonly Role _role;
        private UserRole _userRole;
        private readonly CreateUserByRoleCommand _command;
        private readonly CreateUserByRoleCommandHandler _handler;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<ILogger<CreateUserByRoleCommandHandler>> _loggerMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public CreateUserByRoleCommandHandlerTest()
        {
            _userRepositoryMock = new();
            _roleRepositoryMock = new();
            _loggerMock = new();
            _unitOfWorkMock = new();
            _user = new();
            _role=new() { 
            Id=Guid.NewGuid(),
            Name= "librarian"
            };

            _command = new (_user, _role.Name);
            _handler = new (
                _loggerMock.Object,
                _userRepositoryMock.Object,
                _roleRepositoryMock.Object,
                _unitOfWorkMock.Object
                );

        }
        [Theory]
        [InlineData("ads@gmail.com", "password", "12345", "Test")]
        [InlineData("ads@gmail.com", "password", null, "Test")]
        [InlineData("ads@gmail.com", "password", "12345", null)]
        [InlineData("ads@gmail.com", "password", null, null)]
        public async Task Handle_Should_ReturnUserInfo_WhenSuccess(string Email, string Password, string PhoneNumber, string UserName)
        {

            _user.Id = Guid.NewGuid();
            _user.Email = Email;
            _user.Password = Password;
            _user.PhoneNumber = PhoneNumber;
            _user.UserName = UserName;

            _userRole = new()
            {
                RoleId = _role.Id,
                UserId = _user.Id
            };
            //Arrange


            _roleRepositoryMock.Setup(
               x => x.GetRoleByNameAsync(
                   It.IsAny<string>()))
               .ReturnsAsync(_role);

            _userRepositoryMock.Setup(
               x => x.EmailIsUniqueAsync(
                   It.IsAny<string>()))
               .ReturnsAsync(true);

            _userRepositoryMock.Setup(
               x => x.UserNameIsUniqueAsync(
                   It.IsAny<string>()))
               .ReturnsAsync(true);

            _userRepositoryMock.Setup(
               x => x.PhoneNumberIsUniqueAsync(
                   It.IsAny<string>()))
               .ReturnsAsync(true);

            _userRepositoryMock.Setup(
                x => x.RegisterUserAsync(
                    It.IsAny<User>()))
                .ReturnsAsync(() => _user);

            _userRepositoryMock.Setup(
               x => x.AssignRoleToUserAsync(
                   It.IsAny<UserRole>()));
            

            _unitOfWorkMock.Setup(
               x => x.SaveChangesAsync())
               .ReturnsAsync(1);

            //Act
            var result = await _handler.Handle(_command, default);

            //Assert

            Assert.Equal(_user.Id, result.Id);

            _roleRepositoryMock.Verify(
              x => x.GetRoleByNameAsync(
                  It.Is<string>(a => a == _role.Name)),
              Times.Once);
           

            _userRepositoryMock.Verify(
             x => x.EmailIsUniqueAsync(
                 It.Is<string>(a => a == _user.Email)),
             Times.Once);


            _userRepositoryMock.Verify(
             x => x.UserNameIsUniqueAsync(
                 It.Is<string>(a => a == _user.UserName)),
             Times.Once);

            _userRepositoryMock.Verify(
          x => x.PhoneNumberIsUniqueAsync(
              It.Is<string>(a => a == _user.PhoneNumber)),
          Times.Once);

            _userRepositoryMock.Verify(
                x => x.RegisterUserAsync(It.Is<User>(u => u.Id == result.Id)),
                Times.Once);

            _userRepositoryMock.Verify(
           x => x.AssignRoleToUserAsync(
               It.Is<UserRole>(a=>a.RoleId==_role.Id && a.UserId==_user.Id)),
           Times.Once);

            _unitOfWorkMock.Verify(
               x => x.SaveChangesAsync(),
               Times.Once);

        }
        [Theory]
        [InlineData("ads@gmail.com", "password", "12345", "Test")]
        [InlineData("ads@gmail.com", "password", null, "Test")]
        [InlineData("ads@gmail.com", "password", "12345", null)]
        [InlineData("ads@gmail.com", "password", null, null)]
        public async Task Handle_Should_ThrowCustomNotFoundException_WhenLibrarianRoleNotFound(string Email, string Password, string PhoneNumber, string UserName)
        {
            _user.Id = Guid.NewGuid();
            _user.Email = Email;
            _user.Password = Password;
            _user.PhoneNumber = PhoneNumber;
            _user.UserName = UserName;

            //Arrange
            _roleRepositoryMock.Setup(
               x => x.GetRoleByNameAsync(
                   It.IsAny<string>()))
               .ReturnsAsync(()=>null!);

            //Act
            async Task act() => await _handler.Handle(_command, default);
            CustomNotFoundException ActualException = await Assert.ThrowsAsync<CustomNotFoundException>(act);
            CustomNotFoundException ExpectedException = new($"Role {_role.Name}");

            //Assert   

            _roleRepositoryMock.Verify(
              x => x.GetRoleByNameAsync(
                  It.IsAny<string>()),
              Times.Once);

            _userRepositoryMock.Verify(
          x => x.EmailIsUniqueAsync(
              It.Is<string>(a => a == _user.Email)),
          Times.Never);

            _userRepositoryMock.Verify(
              x => x.RegisterUserAsync(_user),
              Times.Never);

            _userRepositoryMock.Verify(
          x => x.AssignRoleToUserAsync(
              It.Is<UserRole>(a => a.RoleId == _role.Id && a.UserId == _user.Id)),
          Times.Never);

            _unitOfWorkMock.Verify(
               x => x.SaveChangesAsync(),
               Times.Never);
            Assert.Equal(ExpectedException.Message, ActualException.Message);


        }

        [Theory]
        [InlineData("ads@gmail.com", "password", "12345", "Test")]
        [InlineData("ads@gmail.com", "password", null, "Test")]
        [InlineData("ads@gmail.com", "password", "12345", null)]
        [InlineData("ads@gmail.com", "password", null, null)]
        public async Task Handle_Should_ThrowUserExistsException_WhenEmailIsNotUnique(string Email, string Password, string PhoneNumber, string UserName)
        {
            _user.Id = Guid.NewGuid();
            _user.Email = Email;
            _user.Password = Password;
            _user.PhoneNumber = PhoneNumber;
            _user.UserName = UserName;

            //Arrange
            _roleRepositoryMock.Setup(
             x => x.GetRoleByNameAsync(
                 It.IsAny<string>()))
             .ReturnsAsync(_role);

            _userRepositoryMock.Setup(
                x => x.EmailIsUniqueAsync(
                    It.IsAny<string>()))
                .ReturnsAsync(false);

            //Act
            async Task act() => await _handler.Handle(_command, default);
            UserExistsException ActualException = await Assert.ThrowsAsync<UserExistsException>(act);
            UserExistsException ExpectedException = new($"{nameof(_user.Email)}");

            //Assert   
            _roleRepositoryMock.Verify(
            x => x.GetRoleByNameAsync(
                It.Is<string>(a => a == _role.Name)),
            Times.Once);

            _userRepositoryMock.Verify(
          x => x.EmailIsUniqueAsync(
              It.Is<string>(a => a == _user.Email)),
          Times.Once);
            _userRepositoryMock.Verify(
              x => x.RegisterUserAsync(_user),
              Times.Never);

            _userRepositoryMock.Verify(
         x => x.AssignRoleToUserAsync(
             It.Is<UserRole>(a => a.RoleId == _role.Id && a.UserId == _user.Id)),
         Times.Never);

            _unitOfWorkMock.Verify(
               x => x.SaveChangesAsync(),
               Times.Never);
            Assert.Equal(ExpectedException.Message, ActualException.Message);


        }


        [Theory]
        [InlineData("ads@gmail.com", "password", "12345", "Test")]
        [InlineData("ads@gmail.com", "password", null, "Test")]
        [InlineData("ads@gmail.com", "password", "12345", null)]
        [InlineData("ads@gmail.com", "password", null, null)]
        public async Task Handle_Should_ThrowUserExistsException_WhenUserNameIsNotUnique(string Email, string Password, string PhoneNumber, string UserName)
        {
            _user.Id = Guid.NewGuid();
            _user.Email = Email;
            _user.Password = Password;
            _user.PhoneNumber = PhoneNumber;
            _user.UserName = UserName;
            //Arrange

            _roleRepositoryMock.Setup(
              x => x.GetRoleByNameAsync(
                  It.IsAny<string>()))
              .ReturnsAsync(_role);

            _userRepositoryMock.Setup(
              x => x.EmailIsUniqueAsync(
                  It.IsAny<string>()))
              .ReturnsAsync(true);

            _userRepositoryMock.Setup(
             x => x.UserNameIsUniqueAsync(
                 It.IsAny<string>()))
             .ReturnsAsync(false);


            //Act
            async Task act() => await _handler.Handle(_command, default);
            UserExistsException Actual = await Assert.ThrowsAsync<UserExistsException>(act);
            UserExistsException ExpectedException = new($"{nameof(_user.UserName)}");

            //Assert   

            _roleRepositoryMock.Verify(
            x => x.GetRoleByNameAsync(
                It.Is<string>(a => a == _role.Name)),
            Times.Once);

            _userRepositoryMock.Verify(
              x => x.EmailIsUniqueAsync(
                  It.Is<string>(a => a == _user.Email)),
              Times.Once);


            _userRepositoryMock.Verify(
             x => x.UserNameIsUniqueAsync(
                 It.Is<string>(a => a == _user.UserName)),
             Times.Once);

            _userRepositoryMock.Verify(
              x => x.RegisterUserAsync(_user),
              Times.Never);

            _userRepositoryMock.Verify(
         x => x.AssignRoleToUserAsync(
             It.Is<UserRole>(a => a.RoleId == _role.Id && a.UserId == _user.Id)),
         Times.Never);

            _unitOfWorkMock.Verify(
               x => x.SaveChangesAsync(),
               Times.Never);
            Assert.Equal(ExpectedException.Message, Actual.Message);


        }
        [Theory]
        [InlineData("ads@gmail.com", "password", "12345", "Test")]
        [InlineData("ads@gmail.com", "password", null, "Test")]
        [InlineData("ads@gmail.com", "password", "12345", null)]
        [InlineData("ads@gmail.com", "password", null, null)]
        public async Task Handle_Should_ThrowUserExistsException_WhenPhoneNumberIsNotUnique(string Email, string Password, string PhoneNumber, string UserName)
        {
            _user.Id = Guid.NewGuid();
            _user.Email = Email;
            _user.Password = Password;
            _user.PhoneNumber = PhoneNumber;
            _user.UserName = UserName;
            //Arrange

            _roleRepositoryMock.Setup(
             x => x.GetRoleByNameAsync(
                 It.IsAny<string>()))
             .ReturnsAsync(_role);


            _userRepositoryMock.Setup(
              x => x.EmailIsUniqueAsync(
                  It.IsAny<string>()))
              .ReturnsAsync(true);

            _userRepositoryMock.Setup(
               x => x.UserNameIsUniqueAsync(
                   It.IsAny<string>()))
               .ReturnsAsync(true);

            _userRepositoryMock.Setup(
               x => x.PhoneNumberIsUniqueAsync(
                   It.IsAny<string>()))
               .ReturnsAsync(false);

            //Act
            async Task act() => await _handler.Handle(_command, default);
            UserExistsException Actual = await Assert.ThrowsAsync<UserExistsException>(act);
            UserExistsException ExpectedException = new($"{nameof(_user.PhoneNumber)}");

            //Assert   

            _roleRepositoryMock.Verify(
            x => x.GetRoleByNameAsync(
                It.Is<string>(a => a == _role.Name)),
            Times.Once);

            _userRepositoryMock.Verify(
             x => x.EmailIsUniqueAsync(
                 It.Is<string>(a => a == _user.Email)),
             Times.Once);


            _userRepositoryMock.Verify(
             x => x.UserNameIsUniqueAsync(
                 It.Is<string>(a => a == _user.UserName)),
             Times.Once);

            _userRepositoryMock.Verify(
          x => x.PhoneNumberIsUniqueAsync(
              It.Is<string>(a => a == _user.PhoneNumber)),
          Times.Once);
            _userRepositoryMock.Verify(
              x => x.RegisterUserAsync(_user),
              Times.Never);

            _userRepositoryMock.Verify(
         x => x.AssignRoleToUserAsync(
             It.Is<UserRole>(a => a.RoleId == _role.Id && a.UserId == _user.Id)),
         Times.Never);

            _unitOfWorkMock.Verify(
               x => x.SaveChangesAsync(),
               Times.Never);
            Assert.Equal(ExpectedException.Message, Actual.Message);


        }
    }
}
