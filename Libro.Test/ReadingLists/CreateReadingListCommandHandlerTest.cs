using Libro.Application.Interfaces;
using Libro.Application.ReadingLists.Commands;
using Libro.Domain.Entities;
using Libro.Infrastructure;
using Libro.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Libro.Test.ReadingLists
{
    public class CreateReadingListCommandHandlerTest
    {
        private readonly User _user;
        private readonly ReadingList _readingList;
        private readonly CreateReadingListCommand _command;
        private readonly CreateReadingListCommandHandler _handler;
        private readonly Mock<IReadingListRepository> _readingListRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILogger<CreateReadingListCommandHandler>> _loggerMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        public CreateReadingListCommandHandlerTest()
        {

            _user = new()
            {
                Id = Guid.NewGuid(),
                Email = "ads@gmail.com",
                Password = "password",
                PhoneNumber = "12345",
                UserName = "Test"
            };
            _readingList = new()
            {
                Id = Guid.NewGuid(),
                Name = "readingList1"
            };

            _unitOfWorkMock = new();
            _userRepositoryMock = new();
            _readingListRepositoryMock = new();
            _loggerMock = new();
            _command = new(_user.Id, _readingList);
            _handler = new(
                _readingListRepositoryMock.Object,
                _userRepositoryMock.Object,
                _loggerMock.Object,
                _unitOfWorkMock.Object
                );
        }

        [Fact]
        public async Task Handle_Should_ReturnReadingList_WhenReadingListIsCreated()
        {
            //Arrange

            _userRepositoryMock.Setup(
           x => x.GetUserAsync(
               It.IsAny<Guid>()))
                .ReturnsAsync(_user);

            _readingListRepositoryMock.Setup(
              x => x.CreateReadingListAsync(
                  It.IsAny<ReadingList>()));

            _unitOfWorkMock.Setup(
             x => x.SaveChangesAsync())
                .ReturnsAsync(1);
            //Act
            var result = await _handler.Handle(_command, default);

            //Assert

            _userRepositoryMock.Verify(
          x => x.GetUserAsync(
              It.Is<Guid>(a => a == _user.Id)),
          Times.Once);


            _readingListRepositoryMock.Verify(
              x => x.CreateReadingListAsync(It.Is<ReadingList>(x => x.Id == _readingList.Id)),
              Times.Once);

            _unitOfWorkMock.Verify(
              x => x.SaveChangesAsync(),
              Times.Once);

            Assert.Equal(_readingList.Id, result.Id);
        }

    }
}
