using Castle.Components.DictionaryAdapter.Xml;
using Castle.Core.Logging;
using Libro.Application.Interfaces;
using Libro.Application.Users.Commands;
using Libro.Domain.Entities;
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
    public class CreateUserCommandHandlerTest
    {
        private User _user;
        private readonly CreateUserCommand _command;
        private readonly CreateUserCommandHandler _handler;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILogger<CreateUserCommandHandler>> _loggerMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    
        public CreateUserCommandHandlerTest()
        {
            _userRepositoryMock = new();
            _loggerMock = new();
            _unitOfWorkMock = new();
            _user = new();
            _command = new CreateUserCommand(_user);
            _handler = new CreateUserCommandHandler(
                _loggerMock.Object,
                _userRepositoryMock.Object,
                _unitOfWorkMock.Object
                );

        }
        [Theory]
        [InlineData("ads@gmail.com", "password", "12345", "Test")]
        [InlineData("ads@gmail.com", "password", null, "Test")]
        [InlineData("ads@gmail.com", "password", "12345", null)]
        [InlineData("ads@gmail.com", "password", null, null)]
        public async Task Handle_Should_ReturnUserInfo_WhenSuccess(string Email,string Password,string PhoneNumber,string UserName)
        {

            _user.Id = Guid.NewGuid();
            _user.Email = Email;
            _user.Password = Password;
            _user.PhoneNumber = PhoneNumber;
            _user.UserName = UserName;


            //Arrange
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

            _unitOfWorkMock.Setup(
               x => x.SaveChangesAsync())
               .ReturnsAsync(1);

            //Act
            var result = await _handler.Handle(_command, default);

            //Assert
            Assert.Equal(_user.Id,result.Id);

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
                x => x.RegisterUserAsync(It.Is<User>(u=>u.Id==result.Id)),
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
        public async Task Handle_Should_ThrowUserExistsException_WhenEmailIsNotUnique(string Email, string Password, string PhoneNumber, string UserName)
        {
            _user.Id = Guid.NewGuid();
            _user.Email = Email;
            _user.Password = Password;
            _user.PhoneNumber = PhoneNumber;
            _user.UserName = UserName;

            //Arrange
            _userRepositoryMock.Setup(
                x => x.EmailIsUniqueAsync(
                    It.IsAny<string>()))
                .ReturnsAsync(false);
            
            //Act
            async Task act() => await _handler.Handle(_command, default);
            UserExistsException ActualException  = await Assert.ThrowsAsync<UserExistsException>(act);
            UserExistsException ExpectedException = new($"{nameof(_user.Email)}");

            //Assert   
            _userRepositoryMock.Verify(
          x => x.EmailIsUniqueAsync(
              It.Is<string>(a => a == _user.Email)),
          Times.Once);
            _userRepositoryMock.Verify(
              x => x.RegisterUserAsync(_user),
              Times.Never);
            _unitOfWorkMock.Verify(
               x => x.SaveChangesAsync(),
               Times.Never);
            Assert.Equal(ExpectedException.Message, ActualException .Message);
           

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
            //Arrange
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
            _unitOfWorkMock.Verify(
               x => x.SaveChangesAsync(),
               Times.Never);
            Assert.Equal(ExpectedException.Message, Actual.Message);


        }

    }
}

