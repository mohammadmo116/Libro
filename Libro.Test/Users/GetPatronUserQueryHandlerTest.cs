using Libro.Application.Interfaces;
using Libro.Application.Users.Commands;
using Libro.Application.Users.Queries;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Libro.Test.Users
{
    public class GetPatronUserQueryHandlerTest
    {
        private readonly Role _admin;
        private readonly Role _patron;
        private readonly User _user;
        private readonly GetPatronUserQuery _query;
        private readonly GetPatronUserQueryHandler _handler;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILogger<GetPatronUserQueryHandler>> _loggerMock;
        public GetPatronUserQueryHandlerTest() {
            _user = new()
            {
            Id = Guid.NewGuid(),
            Email = "sad@gmail.com",
            Password = "Password",
            PhoneNumber = "PhoneNumber",
            UserName = "UserName"
        };
             _admin = new()
            {
                Id = Guid.NewGuid(),
                Name = "admin",

            };
             _patron = new()
            {
                Id = Guid.NewGuid(),
                Name = "patron",

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
        public async Task Handle_Should_ReturnUser_WhenPatronUserIsFound()
        {

            //Arrange
            _userRepositoryMock.Setup(
                x => x.GetUserAsync(
                    It.IsAny<Guid>()))
                .ReturnsAsync(() => _user);

            _user.Roles.Add(_patron);

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
        [Fact]
        public async Task Handle_Should_ThrowCustomNotFoundException_WhenUserIsNotPatron()
        {

            //Arrange
            _userRepositoryMock.Setup(
                x => x.GetUserAsync(
                    It.IsAny<Guid>()))
                .ReturnsAsync(() => _user);

            _user.Roles.Add(_admin);

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
