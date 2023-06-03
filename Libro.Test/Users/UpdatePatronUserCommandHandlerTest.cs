﻿using Libro.Application.Interfaces;
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
    public class UpdatePatronUserCommandHandlerTest
    {
        private readonly Role _admin;
        private readonly Role _patron;
        private readonly User _user;
        private readonly UpdatePatronUserCommand _command;
        private readonly UpdatePatronUserCommandHandler _handler;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<UpdatePatronUserCommandHandler>> _loggerMock;
        public UpdatePatronUserCommandHandlerTest()
        {
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
            _unitOfWorkMock = new();
            _userRepositoryMock = new();
            _loggerMock = new();
            _command = new(_user);
            _handler = new(
                _loggerMock.Object,
                _userRepositoryMock.Object,
                _unitOfWorkMock.Object
                );
        }
        [Fact]
        public async Task Handle_Should_ReturnTrue_WhenPatronUserIsFoundAndEdited()
        {

            //Arrange
            _userRepositoryMock.Setup(
                x => x.GetUserWtithRolesAsync(
                    It.IsAny<Guid>()))
                .ReturnsAsync(() => _user);

            _userRepositoryMock.Setup(
                x => x.UpdateUser(
                    It.IsAny<User>()));

            _unitOfWorkMock.Setup(
               x => x.SaveChangesAsync())
                .ReturnsAsync(1);


            _user.Roles.Add(_patron);

            //Act
            var result = await _handler.Handle(_command, default);

            //Assert
            _userRepositoryMock.Verify(
              x => x.GetUserWtithRolesAsync(It.Is<Guid>(x => x == _user.Id)),
              Times.Once);
            _userRepositoryMock.Verify(
                x => x.UpdateUser(
                    It.Is<User>(a=>a.Id==_user.Id)),
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
            CustomNotFoundException ExpectedException = new("User");

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
        public async Task Handle_Should_ThrowCustomNotFoundException_WhenUserIsNotPatron()
        {

            //Arrange
            _userRepositoryMock.Setup(
                x => x.GetUserWtithRolesAsync(
                    It.IsAny<Guid>()))
                .ReturnsAsync(() => _user);

            _user.Roles.Add(_admin);

            //Act
            async Task act() => await _handler.Handle(_command, default);
            CustomNotFoundException ActualException = await Assert.ThrowsAsync<CustomNotFoundException>(act);
            CustomNotFoundException ExpectedException = new("User");
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
    }
}
