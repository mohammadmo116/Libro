using Libro.Application.Interfaces;
using Libro.Application.Notifications.Commands;
using Libro.Domain.Entities;
using Libro.Infrastructure;
using Libro.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Assert = Xunit.Assert;

namespace Libro.Test.Notifications
{
    public class NotifyPatronsForReservedBooksCommandHandlerTest
    {
        private readonly User _user;
        private readonly Notification _notification;
        private readonly NotifyPatronsForReservedBooksCommand _command;
        private readonly NotifyPatronsForReservedBooksCommandHandler _handler;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<INotificationRepository> _notificationRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<NotifyPatronsForReservedBooksCommandHandler>> _loggerMock;

        public NotifyPatronsForReservedBooksCommandHandlerTest()
        {

            _user = new()
            {
                Id = Guid.NewGuid(),
                Email = "test@test.com",
                UserName = "test",
                PhoneNumber = "123",
            };
            _notification = new()
            {
                Id = _user.Id,
                Message = "Test",
                UserId = _user.Id,

            };
            _notificationRepositoryMock = new();
            _userRepositoryMock = new();
            _unitOfWorkMock = new();
            _loggerMock = new();
            _command = new();
            _handler = new(
                _notificationRepositoryMock.Object,
                _userRepositoryMock.Object,
                _loggerMock.Object,
                _unitOfWorkMock.Object);

        }
        [Fact]
        public async Task Handle_Should_ReturnTrue_WhenNotifyTheUser()
        {

            //Arrange
            _userRepositoryMock.Setup(x =>
                x.GetPatronIdsForReservedBooksAsync()
                ).ReturnsAsync(new List<Guid>()
                { Guid.NewGuid(),
                    Guid.NewGuid(),

                });

            _notificationRepositoryMock.Setup(x =>
                x.NotifyUsers(
                    It.IsAny<List<string>>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ));

            _notificationRepositoryMock.Setup(x =>
                x.DataBaseNotify(
                    It.IsAny<List<Notification>>()
                    ));

            _unitOfWorkMock.Setup(
                x => x.SaveChangesAsync())
            .ReturnsAsync(1);

            //Act
            var result = await _handler.Handle(_command, default);


            //Assert

            _userRepositoryMock.Verify(x =>
                x.GetPatronIdsForReservedBooksAsync(),
                Times.Once);

            _notificationRepositoryMock.Verify(x =>
                x.NotifyUsers(
                    It.IsAny<List<string>>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                    Times.Once);

            _notificationRepositoryMock.Verify(x =>
                x.DataBaseNotify(
                     It.IsAny<List<Notification>>()),
                    Times.Once);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);

            Assert.True(result);

        }
        [Fact]
        public async Task Handle_ShouldDoNothingAndReturnTrue_WhenUserIdsIsEmpty()
        {

            //Arrange
            _userRepositoryMock.Setup(x =>
                x.GetPatronIdsForReservedBooksAsync()
                ).ReturnsAsync(new List<Guid>());


            //Act
            var result = await _handler.Handle(_command, default);


            //Assert

            Assert.True(result);

            _userRepositoryMock.Verify(x =>
                  x.GetPatronIdsForReservedBooksAsync(),
                  Times.Once);

            _notificationRepositoryMock.Verify(x =>
                x.NotifyUsers(
                    It.IsAny<List<string>>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                    Times.Never);

            _notificationRepositoryMock.Verify(x =>
                x.DataBaseNotify(
                     It.IsAny<List<Notification>>()),
                    Times.Never);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);



        }

    }
}
