using Castle.Components.DictionaryAdapter.Xml;
using Castle.Core.Logging;
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
    public class CreateUserCommandHandlerTest
    {
        private readonly User _user;
        private readonly CreateUserCommand _command;
        private readonly CreateUserCommandHandler _handler;
        private readonly Mock<IAuthenticationRepository> _authenticationRepositoryMock;
        private readonly Mock<ILogger<CreateUserCommandHandler>> _loggerMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    
        public CreateUserCommandHandlerTest()
        {
            _authenticationRepositoryMock = new();
            _loggerMock = new();
            _unitOfWorkMock = new();
            _user = new()
            {
                Id = Guid.NewGuid(),
                Email = "ads@gmail.com",
                Password = "password",
                PhoneNumber = "12345",
                UserName = "Test"
            };
            _command = new CreateUserCommand(_user);
            _handler = new CreateUserCommandHandler(
                _loggerMock.Object,
                _authenticationRepositoryMock.Object,
                _unitOfWorkMock.Object
                );
        }
        [Fact]
        public async Task Handle_Should_ReturnUserInfo_WhenSuccess()
        {
       
           
            //Arrange
            _authenticationRepositoryMock.Setup(
                x => x.RegisterUserAsync(
                    It.IsAny<User>()))
                .ReturnsAsync(() => _user);

            _unitOfWorkMock.Setup(
               x => x.SaveChangesAsync())
               .ReturnsAsync(1);

            //Act
            var result = await _handler.Handle(_command, default);

            //Assert
            _authenticationRepositoryMock.Verify(
                x => x.RegisterUserAsync(It.Is<User>(u => u.Id == result.Id)),
                Times.Once);

            _unitOfWorkMock.Verify(
               x => x.SaveChangesAsync(),
               Times.Once);

        }
        [Fact]
        public async Task Handle_Should_ThrowUserExistsException_WhenEmailIsNotUnique()
        {
        
            //Arrange
            _authenticationRepositoryMock.Setup(
                x =>x.ExceptionIfUserExistsAsync(
                    It.IsAny<User>()))
                .ThrowsAsync(new UserExistsException(nameof(_user.Email)));
            
            //Act
            async Task act() => await _handler.Handle(_command, default);
            UserExistsException Actual = await Assert.ThrowsAsync<UserExistsException>(act);
            UserExistsException ExpectedException = new($"{nameof(_user.Email)}");

            //Assert   
            _authenticationRepositoryMock.Verify(
              x => x.ExceptionIfUserExistsAsync(_user),
              Times.Once);
            _authenticationRepositoryMock.Verify(
              x => x.RegisterUserAsync(_user),
              Times.Never);
            _unitOfWorkMock.Verify(
               x => x.SaveChangesAsync(),
               Times.Never);
            Assert.Equal(ExpectedException.Message, Actual.Message);
           

        }

        [Fact]
        public async Task Handle_Should_ThrowUserExistsException_WhenUserNameIsNotUnique()
        {

            //Arrange
            //Arrange
            _authenticationRepositoryMock.Setup(
                x => x.ExceptionIfUserExistsAsync(
                    It.IsAny<User>()))
                .ThrowsAsync(new UserExistsException(nameof(_user.UserName)));

            //Act
            async Task act() => await _handler.Handle(_command, default);
            UserExistsException Actual = await Assert.ThrowsAsync<UserExistsException>(act);
            UserExistsException ExpectedException = new($"{nameof(_user.UserName)}");

            //Assert   
            _authenticationRepositoryMock.Verify(
            x => x.ExceptionIfUserExistsAsync(_user),
            Times.Once);
            _authenticationRepositoryMock.Verify(
              x => x.RegisterUserAsync(_user),
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
            _authenticationRepositoryMock.Setup(
           x => x.ExceptionIfUserExistsAsync(
               It.IsAny<User>()))
           .ThrowsAsync(new UserExistsException(nameof(_user.PhoneNumber)));

            //Act
            async Task act() => await _handler.Handle(_command, default);
            UserExistsException Actual = await Assert.ThrowsAsync<UserExistsException>(act);
            UserExistsException ExpectedException = new($"{nameof(_user.PhoneNumber)}");

            //Assert   
            _authenticationRepositoryMock.Verify(
              x => x.ExceptionIfUserExistsAsync(_user),
              Times.Once);
            _authenticationRepositoryMock.Verify(
              x => x.RegisterUserAsync(_user),
              Times.Never);
            _unitOfWorkMock.Verify(
               x => x.SaveChangesAsync(),
               Times.Never);
            Assert.Equal(ExpectedException.Message, Actual.Message);


        }

    }
}

