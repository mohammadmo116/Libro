using Libro.Application.Interfaces;
using Libro.Application.ReadingLists.Commands;
using Libro.Domain.Entities;
using Libro.Infrastructure.Repositories;
using Libro.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libro.Domain.Exceptions;

namespace Libro.Test.ReadingLists
{
    public class UpdateReadingListCommandHandlerTest
    {
        private readonly User _user;
        private readonly ReadingList _readingList;
        private readonly UpdateReadingListCommand _command;
        private readonly UpdateReadingListCommandHandler _handler;
        private readonly Mock<IReadingListRepository> _readingListRepositoryMock;
        private readonly Mock<ILogger<UpdateReadingListCommandHandler>> _loggerMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        public UpdateReadingListCommandHandlerTest()
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
            _readingListRepositoryMock = new();
            _loggerMock = new();
            _command = new(_user.Id,_readingList);
            _handler = new(
                _readingListRepositoryMock.Object,
                _loggerMock.Object,
                _unitOfWorkMock.Object
                );
        }

        [Fact]
        public async Task Handle_Should_ReturnTrue_WhenReadingListIsFoundAndEdited()
        {

            //Arrange
            _readingListRepositoryMock.Setup(
                x => x.GetReadingListByUserAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>()))
                .ReturnsAsync(() => _readingList);

            _readingListRepositoryMock.Setup(
              x => x.UpdateReadingList(
                  It.IsAny<ReadingList>()));

            _unitOfWorkMock.Setup(
               x => x.SaveChangesAsync())
                .ReturnsAsync(1);


            //Act
            var result = await _handler.Handle(_command, default);

            //Assert
            _readingListRepositoryMock.Verify(
              x => x.GetReadingListByUserAsync(
                  It.Is<Guid>(x => x == _user.Id),
                  It.Is<Guid>(x => x == _readingList.Id)
                  ),
              Times.Once);

            _readingListRepositoryMock.Verify(
             x => x.UpdateReadingList(
                 It.Is<ReadingList>(b => b.Id == _readingList.Id)),
             Times.Once);


            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);

            Assert.True(result);

        }
        [Fact]
        public async Task Handle_Should_ThrowCustomNotFoundException_WhenReadingListIsNotFound()
        {

            //Arrange
            _readingListRepositoryMock.Setup(
                 x => x.GetReadingListByUserAsync(
                     It.IsAny<Guid>(),
                     It.IsAny<Guid>()))
                 .ReturnsAsync(() => null!);

            //Act
            async Task act() => await _handler.Handle(_command, default);
            CustomNotFoundException ActualException = await Assert.ThrowsAsync<CustomNotFoundException>(act);
            CustomNotFoundException ExpectedException = new("ReadingList");

            //Assert
            _readingListRepositoryMock.Verify(
           x => x.GetReadingListByUserAsync(
               It.Is<Guid>(x => x == _user.Id),
               It.Is<Guid>(x => x == _readingList.Id)
               ),
           Times.Once);

            _readingListRepositoryMock.Verify(
           x => x.UpdateReadingList(
               It.Is<ReadingList>(b => b.Id == _readingList.Id)),
           Times.Never);


            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
            Assert.Equal(ExpectedException.Message, ActualException.Message);

        }

    }
}
