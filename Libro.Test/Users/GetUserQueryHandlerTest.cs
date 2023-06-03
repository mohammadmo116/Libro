using Libro.Application.Interfaces;
using Libro.Application.Users.Queries;
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
    public class GetUserQueryHandlerTest
    {
        private readonly User _user;
        private readonly GetUserQuery _query;
        private readonly GetUserQueryHandler _handler;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILogger<GetUserQueryHandler>> _loggerMock;
        public GetUserQueryHandlerTest()
        {
            _user = new()
            {
                Id = Guid.NewGuid(),
                Email = "sad@gmail.com",
                Password = "Password",
                PhoneNumber = "PhoneNumber",
                UserName = "UserName"
            };
  
            _userRepositoryMock = new();
            _loggerMock = new();
            _query = new(_user.Id);
            _handler = new(
                _loggerMock.Object,
                _userRepositoryMock.Object
                );
        }

        [Fact]
        public async Task Handle_Should_ReturnUser_WhenUserIsFound()
        {

            //Arrange
            _userRepositoryMock.Setup(
                x => x.GetUserAsync(
                    It.IsAny<Guid>()))
                .ReturnsAsync(() => _user);

            //Act
            var result = await _handler.Handle(_query, default);

            //Assert
            _userRepositoryMock.Verify(
              x => x.GetUserAsync(It.Is<Guid>(x => x == _user.Id)),
              Times.Once);
            Assert.Equal(_user.Id, result.Id);

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
            async Task act() => await _handler.Handle(_query, default);
            CustomNotFoundException ActualException = await Assert.ThrowsAsync<CustomNotFoundException>(act);
            CustomNotFoundException ExpectedException = new("User");

            //Assert
            _userRepositoryMock.Verify(
              x => x.GetUserAsync(It.Is<Guid>(x => x == _user.Id)),
              Times.Once);
            Assert.Equal(ExpectedException.Message, ActualException.Message);

        }
    }
}
