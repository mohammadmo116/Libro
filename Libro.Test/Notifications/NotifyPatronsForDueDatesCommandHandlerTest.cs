using FluentEmail.Core;
using FluentEmail.Core.Models;
using Libro.Application.Interfaces;
using Libro.Application.Notifications.Commands;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Infrastructure;
using Libro.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Libro.Test.Notifications
{
    public class NotifyPatronsForDueDatesCommandHandlerTest
    {
       
        private readonly List<User> _userlist = new();
        private readonly Book _book;
        private readonly List<Book> _booklist = new();
        private readonly Role _patron;
        private readonly User _user;
        private readonly Notification _notification;
        private readonly NotifyPatronsForDueDatesCommand _command;
        private readonly NotifyPatronsForDueDatesCommandHandler _handler;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<INotificationRepository> _notificationRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IFluentEmailFactory> _emailFactoryMock;
        private readonly Mock<ILogger<NotifyPatronsForDueDatesCommandHandler>> _loggerMock;

        public NotifyPatronsForDueDatesCommandHandlerTest()
        {

           
            _patron = new()
            {
                Id = Guid.NewGuid(),
                Name = "patron"
            };
            _book = new()
            {
                Id = Guid.NewGuid(),
                Title = "sd",

            };
            _booklist.Add(_book);

            _user = new()
            {
                Id = Guid.NewGuid(),
                Email = "test@test.com",
                UserName = "test",
                PhoneNumber = "123",
                Books = _booklist

            };
            _user.Roles.Add(_patron);
            _notification = new()
            {
                Id = _user.Id,
                Message = "Test",
                UserId = _user.Id,

            };

            var transaction = new BookTransaction()
            {

                UserId = _user.Id,
                BookId = _book.Id,
                Status = BookStatus.Borrowed,
                Id = Guid.NewGuid(),
                Book = _book,
                DueDate = DateTime.UtcNow.AddDays(5)

            };

            _userlist.Add(_user);
            _user.BookTransactions.Add(transaction);
            _notificationRepositoryMock = new();
            _userRepositoryMock = new();
            _unitOfWorkMock = new();
            _loggerMock = new();
            _emailFactoryMock=new();
            _command = new();
            _handler = new(
                _notificationRepositoryMock.Object,
                _userRepositoryMock.Object,
                _loggerMock.Object,
                _unitOfWorkMock.Object,
                _emailFactoryMock.Object);

        }

        [Fact]
        public async Task Handle_Should_ReturnTrue_WhenNotifyTheUser()
        {

            //Arrange
            _userRepositoryMock.Setup(x =>
                x.GetPatronsWithDueDatesAsync()
                ).ReturnsAsync(_userlist);

            Mock<IFluentEmail> mockFluentEmail = new();
            _emailFactoryMock.Setup(x => x.Create()).Returns(mockFluentEmail.Object);
            mockFluentEmail.Setup(x => x.To(It.IsAny<string>())).Returns(mockFluentEmail.Object);
            mockFluentEmail.Setup(x => x.Subject(It.IsAny<string>())).Returns(mockFluentEmail.Object);
            mockFluentEmail.Setup(x => x.UsingTemplate(It.IsAny<string>(), It.IsAny<It.IsAnyType>(), It.IsAny<bool>())).Returns(mockFluentEmail.Object);
            mockFluentEmail.Setup(x => x.SendAsync(null)).ReturnsAsync(It.IsAny<SendResponse>());





            _notificationRepositoryMock.Setup(x =>
                x.NotifyUser(
                    It.IsAny<string>(),
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
                x.GetPatronsWithDueDatesAsync(),
                Times.Once);

            _notificationRepositoryMock.Verify(x =>
                x.NotifyUser(
                    It.IsAny<string>(),
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
                x.GetPatronsWithDueDatesAsync()
                ).ReturnsAsync(new List<User>());


            //Act
            var result = await _handler.Handle(_command, default);


            //Assert

            Assert.True(result);

            _userRepositoryMock.Verify(x =>
                  x.GetPatronsWithDueDatesAsync(),
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
