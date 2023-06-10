using Libro.Application.Interfaces;
using Libro.Application.Notifications.Queries;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Assert = Xunit.Assert;

namespace Libro.Test.Notifications
{
    public class GetUserNotificaionsQueryHandlerTest
    {
        private readonly User _user;
        private readonly List<Notification> _notificationsList;
        private readonly GetUserNotificaionsQuery _query;
        private readonly GetUserNotificaionsQueryHandler _handler;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<INotificationRepository> _notificationRepositoryMock;
        private readonly Mock<ILogger<GetUserNotificaionsQueryHandler>> _loggerMock;

        public GetUserNotificaionsQueryHandlerTest()
        {

            _user = new()
            {
                Id = Guid.NewGuid(),
                Email = "test@test.com",
                UserName = "test",
                PhoneNumber = "123",
            };
            _notificationsList = new() {

            new()
            {
                Id = _user.Id,
                Message = "Test1",
                UserId = _user.Id,

            },
               new()
            {
                Id = _user.Id,
                Message = "Test2",
                UserId = _user.Id,

            },
            };

            _notificationRepositoryMock = new();
            _userRepositoryMock = new();
            _loggerMock = new();
            _query = new(_user.Id, 1, 1);
            _handler = new(
                _notificationRepositoryMock.Object,
                _userRepositoryMock.Object,
                _loggerMock.Object);

        }

        [Fact]
        public async Task Handle_Should_ReturnNotifications_WhenUserIsFound()
        {
            //Arrange

            _userRepositoryMock.Setup(
             x => x.GetUserAsync(
                 It.IsAny<Guid>()))
               .ReturnsAsync(_user);

            _notificationRepositoryMock.Setup(
              x => x.GetNotifications(
                  It.IsAny<Guid>(),
                  It.IsAny<int>(),
                 It.IsAny<int>()))
                .ReturnsAsync((_notificationsList, 1));

            //Act
            var result = await _handler.Handle(_query, default);

            //Assert  

            _userRepositoryMock.Verify(
              x => x.GetUserAsync(
                  It.Is<Guid>(a => a == _user.Id)),
              Times.Once);

            _notificationRepositoryMock.Verify(
              x => x.GetNotifications(
                  It.Is<Guid>(a => a == _user.Id),
                  It.Is<int>(a => a == 1),
                  It.Is<int>(a => a == 1)),
              Times.Once);


            CollectionAssert.AreEqual(_notificationsList, result.Item1);
            Assert.Equal(1, result.Item2);
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
           x => x.GetUserAsync(
               It.Is<Guid>(a => a == _user.Id)),
           Times.Once);

            _notificationRepositoryMock.Verify(
             x => x.GetNotifications(
                 It.Is<Guid>(a => a == _user.Id),
                 It.Is<int>(a => a == 1),
                 It.Is<int>(a => a == 1)),
             Times.Never);

            Assert.Equal(ExpectedException.Message, ActualException.Message);

        }
    }
}
