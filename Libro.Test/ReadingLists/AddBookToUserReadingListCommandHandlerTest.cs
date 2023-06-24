using Libro.Application.Interfaces;
using Libro.Application.ReadingLists.Commands;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Exceptions.ReadingListExceptions;
using Libro.Infrastructure;
using Libro.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace Libro.Test.ReadingLists
{
    public class AddBookToUserReadingListCommandHandlerTest
    {
        private readonly User _user;
        private readonly Book _book;
        private readonly BookReadingList _bookReadingList;
        private readonly ReadingList _readingList;
        private readonly AddBookToUserReadingListCommand _command;
        private readonly AddBookToUserReadingListCommandHandler _handler;
        private readonly Mock<IReadingListRepository> _readingListRepositoryMock;
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<ILogger<AddBookToUserReadingListCommandHandler>> _loggerMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        public AddBookToUserReadingListCommandHandlerTest()
        {
            _book = new()
            {
                Id = Guid.NewGuid(),
                Genre = "g",
                IsAvailable = true,
                PublishedDate = DateTime.Now,
                Title = "t"
            };
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
            _bookReadingList = new()
            {
                BookId = _book.Id,
                ReadingListId = _readingList.Id
            };
            _unitOfWorkMock = new();
            _bookRepositoryMock = new();
            _readingListRepositoryMock = new();
            _loggerMock = new();
            _command = new(_user.Id, _bookReadingList);
            _handler = new(
                _readingListRepositoryMock.Object,
                _bookRepositoryMock.Object,
                _loggerMock.Object,
                _unitOfWorkMock.Object
                );
        }
        [Fact]
        public async Task Handle_Should_ReturnTrue_WhenBookIsAddedToTheReadingList()
        {
            //Arrange

            _bookRepositoryMock.Setup(
           x => x.GetBookAsync(
               It.IsAny<Guid>()))
                .ReturnsAsync(_book);

            _readingListRepositoryMock.Setup(
              x => x.GetReadingListByUserAsync(
                  It.IsAny<Guid>(),
                  It.IsAny<Guid>()))
                .ReturnsAsync(_readingList);

            _readingListRepositoryMock.Setup(
              x => x.ContainsTheBook(
                  It.IsAny<BookReadingList>()))
                 .ReturnsAsync(false);

            _readingListRepositoryMock.Setup(
             x => x.AddBookToReadingList(
                 It.IsAny<BookReadingList>()));

            _unitOfWorkMock.Setup(
             x => x.SaveChangesAsync())
                .ReturnsAsync(1);
            //Act
            var result = await _handler.Handle(_command, default);

            //Assert
            Assert.True(result);

            _bookRepositoryMock.Verify(
          x => x.GetBookAsync(
              It.Is<Guid>(a => a == _book.Id)),
          Times.Once);


            _readingListRepositoryMock.Verify(
              x => x.GetReadingListByUserAsync(
                  It.Is<Guid>(x => x == _user.Id),
                  It.Is<Guid>(x => x == _readingList.Id)
                  ),
              Times.Once);

            _readingListRepositoryMock.Verify(
              x => x.ContainsTheBook(
                  It.Is<BookReadingList>(x => x.ReadingListId == _readingList.Id && x.BookId == _book.Id)
                  ),
              Times.Once);

            _readingListRepositoryMock.Verify(
              x => x.AddBookToReadingList(
                  It.Is<BookReadingList>(x => x.ReadingListId == _readingList.Id && x.BookId == _book.Id)
                  ),
              Times.Once);

            _unitOfWorkMock.Verify(
              x => x.SaveChangesAsync(),
              Times.Once);


        }
        [Fact]
        public async Task Handle_Should_ShouldThrowCustomNotFoundException_WhenBookIsNotFound()
        {
            //Arrange

            _bookRepositoryMock.Setup(
           x => x.GetBookAsync(
               It.IsAny<Guid>()))
                .ReturnsAsync(() => null!);

            //Act
            async Task act() => await _handler.Handle(_command, default);
            CustomNotFoundException ActualException = await Assert.ThrowsAsync<CustomNotFoundException>(act);
            CustomNotFoundException ExpectedException = new("Book");

            //Assert
            Assert.Equal(ExpectedException.Message, ActualException.Message);

            _bookRepositoryMock.Verify(
          x => x.GetBookAsync(
              It.Is<Guid>(a => a == _book.Id)),
          Times.Once);


            _readingListRepositoryMock.Verify(
              x => x.GetReadingListByUserAsync(
                  It.IsAny<Guid>(),
                  It.IsAny<Guid>()
                  ),
              Times.Never);

            _readingListRepositoryMock.Verify(
              x => x.ContainsTheBook(
                  It.IsAny<BookReadingList>()
                  ),
              Times.Never);

            _readingListRepositoryMock.Verify(
              x => x.AddBookToReadingList(
                  It.IsAny<BookReadingList>()
                  ),
              Times.Never);

            _unitOfWorkMock.Verify(
              x => x.SaveChangesAsync(),
              Times.Never);


        }

        [Fact]
        public async Task Handle_Should_ShouldThrowCustomNotFoundException_WhenReadingListIsNotFound()
        {
            //Arrange

            _bookRepositoryMock.Setup(
           x => x.GetBookAsync(
               It.IsAny<Guid>()))
                .ReturnsAsync(_book);

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
            Assert.Equal(ExpectedException.Message, ActualException.Message);

            _bookRepositoryMock.Verify(
          x => x.GetBookAsync(
              It.Is<Guid>(a => a == _book.Id)),
          Times.Once);


            _readingListRepositoryMock.Verify(
              x => x.GetReadingListByUserAsync(
                  It.IsAny<Guid>(),
                  It.IsAny<Guid>()
                  ),
              Times.Once);

            _readingListRepositoryMock.Verify(
              x => x.ContainsTheBook(
                  It.IsAny<BookReadingList>()
                  ),
              Times.Never);

            _readingListRepositoryMock.Verify(
              x => x.AddBookToReadingList(
                  It.IsAny<BookReadingList>()
                  ),
              Times.Never);

            _unitOfWorkMock.Verify(
              x => x.SaveChangesAsync(),
              Times.Never);


        }

        [Fact]
        public async Task Handle_Should_ShouldThrowReadingListHasTheSpecificBookException_WhenReadingListContainsTheBook()
        {
            //Arrange

            _bookRepositoryMock.Setup(
           x => x.GetBookAsync(
               It.IsAny<Guid>()))
                .ReturnsAsync(_book);

            _readingListRepositoryMock.Setup(
             x => x.GetReadingListByUserAsync(
                 It.IsAny<Guid>(),
                 It.IsAny<Guid>()))
               .ReturnsAsync(_readingList);

            _readingListRepositoryMock.Setup(
        x => x.ContainsTheBook(
            It.IsAny<BookReadingList>()))
           .ReturnsAsync(true);
            //Act
            async Task act() => await _handler.Handle(_command, default);
            ReadingListContainsTheBookException ActualException = await Assert.ThrowsAsync<ReadingListContainsTheBookException>(act);
            ReadingListContainsTheBookException ExpectedException = new();

            //Assert
            Assert.Equal(ExpectedException.Message, ActualException.Message);

            _bookRepositoryMock.Verify(
          x => x.GetBookAsync(
              It.Is<Guid>(a => a == _book.Id)),
          Times.Once);


            _readingListRepositoryMock.Verify(
              x => x.GetReadingListByUserAsync(
                  It.IsAny<Guid>(),
                  It.IsAny<Guid>()
                  ),
              Times.Once);

            _readingListRepositoryMock.Verify(
              x => x.ContainsTheBook(
                  It.IsAny<BookReadingList>()
                  ),
              Times.Once);

            _readingListRepositoryMock.Verify(
              x => x.AddBookToReadingList(
                  It.IsAny<BookReadingList>()
                  ),
              Times.Never);

            _unitOfWorkMock.Verify(
              x => x.SaveChangesAsync(),
              Times.Never);


        }
    }
}
