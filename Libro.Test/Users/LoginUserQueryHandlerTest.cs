using Libro.Application.Interfaces;
using Libro.Application.Users.Commands;
using Libro.Application.Users.Queries;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Test.Users
{
    public class LoginUserQueryHandlerTest
    {
        private readonly User _user;
        private readonly string _Email;
        private readonly string _Password;
        private readonly LoginUserQuery _command;
        private readonly LoginUserQueryHandler _handler;
        private readonly Mock<IAuthenticationRepository> _authenticationRepositoryMock;
        private readonly Mock<ILogger<LoginUserQueryHandler>> _loggerMock;
        public LoginUserQueryHandlerTest()
        {
            _authenticationRepositoryMock = new();
            _loggerMock = new();

            _user = new()
            {
                Id = Guid.NewGuid(),
                Email = "ads@gmail.com",
                Password = "password",
                PhoneNumber = "12345",
                UserName = "Test"
            };

            _Email = "ads@gmail.com";
            _Password = "password";
   
            _command = new (_Email, _Password);
            _handler = new (
                _loggerMock.Object,
                _authenticationRepositoryMock.Object
                );
        }
        [Fact]
        public async Task Handle_Should_ReturnJWT_WhenSuccessLogin()
        {

            //Arrange
            _authenticationRepositoryMock.Setup(
                x => x.ValidateUserCredentialsAsync(
                    It.IsAny<string>(),
                     It.IsAny<string>()))
                .ReturnsAsync(_user);

            _authenticationRepositoryMock.Setup(
             x => x.Authenticate(
                 It.IsAny<User>()))
             .ReturnsAsync("JwtTest");
            //Act
            var result = await _handler.Handle(_command, default);

            //Assert
            _authenticationRepositoryMock.Verify(
                x => x.ValidateUserCredentialsAsync(_Email,_Password),
                Times.Once);
            _authenticationRepositoryMock.Verify(
               x => x.Authenticate(_user),
               Times.Once);
            Assert.Equal("JwtTest",result);


        }
        
        [Fact]
        public async Task Handle_Should_ThrowInvalidCredentialException_WhenInvalidCredentials()
        {

            //Arrange
            var message = $"Invalid Credentials,\n email : {_Email} \n password: {_Password}";

            _authenticationRepositoryMock.Setup(
                x => x.ValidateUserCredentialsAsync(
                    It.IsAny<string>(),
                     It.IsAny<string>()))
                .ThrowsAsync(new InvalidCredentialException(message));

            //Act
            async Task act() => await _handler.Handle(_command, default);
            InvalidCredentialException Actual = await Assert.ThrowsAsync<InvalidCredentialException>(act);
            InvalidCredentialException ExpectedException = new(message);


            //Assert   
            _authenticationRepositoryMock.Verify(
              x => x.ValidateUserCredentialsAsync(_Email,_Password),
              Times.Once);
            _authenticationRepositoryMock.Verify(
            x => x.Authenticate(_user),
            Times.Never);
            Assert.Equal(Actual.Message, ExpectedException.Message);


        }
        
    }
}
