using Libro.Application.BookReviews.Commands;
using Libro.Application.Books.Commands;
using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Exceptions.BookExceptions;
using Libro.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Test.BookReviews
{
    public class CreateBookReviewCommandHandlerTest
    {
        private readonly User _user;
        private readonly Book _book;
        private readonly BookReview _bookReview;
        private readonly CreateBookReviewCommand _command;
        private readonly CreateBookReviewCommandHandler _handler;
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IBookTransactionRepository> _bookTransactionRepositoryMock;
        private readonly Mock<ILogger<CreateBookReviewCommandHandler>> _loggerMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        public CreateBookReviewCommandHandlerTest()
        {
            
            _user = new()
            {
                Id = Guid.NewGuid(),
                Email = "sad@gmail.com",
                Password = "Password",
                PhoneNumber = "PhoneNumber",
                UserName = "UserName"
            };

            _book = new()
            {
                Id = Guid.NewGuid(),
                Title = "title",
                IsAvailable = true,
                Genre = "genre"
            };

            _bookReview = new()
            {
               UserId= _user.Id,
               BookId=_book.Id,
               Rate=5,
               Review="Greate"

            };
            _bookTransactionRepositoryMock = new();
            _userRepositoryMock = new();
            _unitOfWorkMock = new();
            _bookRepositoryMock = new();
            _loggerMock = new();
            _command = new(_bookReview);
            _handler = new(
                _loggerMock.Object,
                _bookRepositoryMock.Object,
                _userRepositoryMock.Object,
                _bookTransactionRepositoryMock.Object,
                _unitOfWorkMock.Object
                );
        }
        [Fact]
        public async Task Handle_Should_ReturnBookReview_WhenBookReviewIsCreated()
        {
            //Arrange
           
             _userRepositoryMock.Setup(
              x => x.GetUserAsync(
                  It.IsAny<Guid>()))
                .ReturnsAsync(_user);

            _bookRepositoryMock.Setup(
              x => x.GetBookAsync(
                  It.IsAny<Guid>()))
                .ReturnsAsync(_book);

            _bookTransactionRepositoryMock.Setup(
             x => x.BookIsReturnedAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>()))
                .ReturnsAsync(() => true);

            _bookRepositoryMock.Setup(
              x => x.BookIsReviewedByUser(
                  It.IsAny<Guid>(),
                  It.IsAny<Guid>()))
                .ReturnsAsync(false);

            _bookRepositoryMock.Setup(
              x => x.CreateReviewAsync(
                  It.IsAny<BookReview>()));

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

            _bookRepositoryMock.Verify(
              x => x.GetBookAsync(
                  It.Is<Guid>(a=>a==_book.Id)),
              Times.Once);

            _bookTransactionRepositoryMock.Verify(
          x => x.BookIsReturnedAsync(
             It.Is<Guid>(a => a == _user.Id),
             It.Is<Guid>(a => a == _book.Id)),
             Times.Once);

            _bookRepositoryMock.Verify(
              x => x.BookIsReviewedByUser(
                  It.Is<Guid>(a => a  == _bookReview.UserId),
                  It.Is<Guid>(a => a == _bookReview.BookId)),
              Times.Once);

            _bookRepositoryMock.Verify(
              x => x.CreateReviewAsync(It.Is<BookReview>(x => x.UserId == _bookReview.UserId && x.BookId==_bookReview.BookId)),
              Times.Once);

            _unitOfWorkMock.Verify(
              x => x.SaveChangesAsync(),
              Times.Once);

            Assert.Equal(_book.Id, result.BookId);
            Assert.Equal(_user.Id, result.UserId);
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
            async Task act() => await _handler.Handle(_command, default);
            CustomNotFoundException ActualException = await Assert.ThrowsAsync<CustomNotFoundException>(act);
            CustomNotFoundException ExpectedException = new("User");

            //Assert

            _userRepositoryMock.Verify(
              x => x.GetUserAsync(
                  It.Is<Guid>(a => a == _user.Id)),
              Times.Once);

            _bookRepositoryMock.Verify(
              x => x.GetBookAsync(
                  It.Is<Guid>(a => a == _book.Id)),
              Times.Never);

            _bookTransactionRepositoryMock.Verify(
          x => x.BookIsReturnedAsync(
             It.Is<Guid>(a => a == _user.Id),
             It.Is<Guid>(a => a == _book.Id)),
             Times.Never);

            _bookRepositoryMock.Verify(
              x => x.BookIsReviewedByUser(
                  It.Is<Guid>(a => a == _bookReview.UserId),
                  It.Is<Guid>(a => a == _bookReview.BookId)),
              Times.Never);

            _bookRepositoryMock.Verify(
              x => x.CreateReviewAsync(It.Is<BookReview>(x => x.UserId == _bookReview.UserId && x.BookId == _bookReview.BookId)),
              Times.Never);

            _unitOfWorkMock.Verify(
              x => x.SaveChangesAsync(),
              Times.Never);
            Assert.Equal(ExpectedException.Message, ActualException.Message);

        }
        [Fact]
        public async Task Handle_Should_ThrowCustomNotFoundException_WhenBookIsNotFound()
        {

            //Arrange


            _userRepositoryMock.Setup(
             x => x.GetUserAsync(
                 It.IsAny<Guid>()))
               .ReturnsAsync(_user);

            _bookRepositoryMock.Setup(
              x => x.GetBookAsync(
                  It.IsAny<Guid>()))
                .ReturnsAsync(() => null!);


            //Act
            async Task act() => await _handler.Handle(_command, default);
            CustomNotFoundException ActualException = await Assert.ThrowsAsync<CustomNotFoundException>(act);
            CustomNotFoundException ExpectedException = new("Book");

            //Assert

            _userRepositoryMock.Verify(
              x => x.GetUserAsync(
                  It.Is<Guid>(a => a == _user.Id)),
              Times.Once);

            _bookRepositoryMock.Verify(
              x => x.GetBookAsync(
                  It.Is<Guid>(a => a == _book.Id)),
              Times.Once);

            _bookTransactionRepositoryMock.Verify(
             x => x.BookIsReturnedAsync(
                It.Is<Guid>(a => a == _user.Id),
                It.Is<Guid>(a => a == _book.Id)),
                Times.Never);


            _bookRepositoryMock.Verify(
              x => x.BookIsReviewedByUser(
                  It.Is<Guid>(a => a == _bookReview.UserId),
                  It.Is<Guid>(a => a == _bookReview.BookId)),
              Times.Never);

            _bookRepositoryMock.Verify(
              x => x.CreateReviewAsync(It.Is<BookReview>(x => x.UserId == _bookReview.UserId && x.BookId == _bookReview.BookId)),
              Times.Never);

            _unitOfWorkMock.Verify(
              x => x.SaveChangesAsync(),
              Times.Never);
            Assert.Equal(ExpectedException.Message, ActualException.Message);

        }
        [Fact]
        public async Task Handle_Should_ThrowNotAllowedToReviewBookException_WhenBookIsNotBorrowedOrReturned()
        {

            //Arrange


            _userRepositoryMock.Setup(
             x => x.GetUserAsync(
                 It.IsAny<Guid>()))
               .ReturnsAsync(_user);

            _bookRepositoryMock.Setup(
              x => x.GetBookAsync(
                  It.IsAny<Guid>()))
                .ReturnsAsync(() => _book);

            _bookTransactionRepositoryMock.Setup(
             x => x.BookIsReturnedAsync(
                 It.IsAny<Guid>(),
                 It.IsAny<Guid>()))
               .ReturnsAsync(() => false);

            //Act
            async Task act() => await _handler.Handle(_command, default);
            NotAllowedToReviewBookException ActualException = await Assert.ThrowsAsync<NotAllowedToReviewBookException>(act);
            NotAllowedToReviewBookException ExpectedException = new();

            //Assert

            _userRepositoryMock.Verify(
              x => x.GetUserAsync(
                  It.Is<Guid>(a => a == _user.Id)),
              Times.Once);

            _bookRepositoryMock.Verify(
              x => x.GetBookAsync(
                  It.Is<Guid>(a => a == _book.Id)),
              Times.Once);

            _bookTransactionRepositoryMock.Verify(
            x => x.BookIsReturnedAsync(
                It.Is<Guid>(a => a == _user.Id),
                It.Is<Guid>(a => a == _book.Id)),
            Times.Once);
          


            _bookRepositoryMock.Verify(
              x => x.BookIsReviewedByUser(
                  It.Is<Guid>(a => a == _bookReview.UserId),
                  It.Is<Guid>(a => a == _bookReview.BookId)),
              Times.Never);

            _bookRepositoryMock.Verify(
              x => x.CreateReviewAsync(It.Is<BookReview>(x => x.UserId == _bookReview.UserId && x.BookId == _bookReview.BookId)),
              Times.Never);

            _unitOfWorkMock.Verify(
              x => x.SaveChangesAsync(),
              Times.Never);
            Assert.Equal(ExpectedException.Message, ActualException.Message);

        }
        [Fact]
        public async Task Handle_Should_ThrowBookIsAlreadyReviewedException_WhenUserHasReviewedTheBook()
        {

            //Arrange

            _userRepositoryMock.Setup(
         x => x.GetUserAsync(
             It.IsAny<Guid>()))
           .ReturnsAsync(() => _user);


            _bookRepositoryMock.Setup(
              x => x.GetBookAsync(
                  It.IsAny<Guid>()))
                .ReturnsAsync(() => _book);

            _bookTransactionRepositoryMock.Setup(
           x => x.BookIsReturnedAsync(
               It.IsAny<Guid>(),
               It.IsAny<Guid>()))
             .ReturnsAsync(() => true);


            _bookRepositoryMock.Setup(
         x => x.BookIsReviewedByUser(
             It.IsAny<Guid>(),
             It.IsAny<Guid>()))
           .ReturnsAsync(true);

            //Act
            async Task act() => await _handler.Handle(_command, default);
            BookIsAlreadyReviewedException ActualException = await Assert.ThrowsAsync<BookIsAlreadyReviewedException>(act);
            BookIsAlreadyReviewedException ExpectedException = new();

            //Assert

            _userRepositoryMock.Verify(
              x => x.GetUserAsync(
                  It.Is<Guid>(a => a == _user.Id)),
              Times.Once);

            _bookRepositoryMock.Verify(
              x => x.GetBookAsync(
                  It.Is<Guid>(a => a == _book.Id)),
              Times.Once);

            _bookTransactionRepositoryMock.Verify(
           x => x.BookIsReturnedAsync(
               It.Is<Guid>(a => a == _user.Id),
               It.Is<Guid>(a => a == _book.Id)),
           Times.Once);

            _bookRepositoryMock.Verify(
              x => x.BookIsReviewedByUser(
                  It.Is<Guid>(a => a == _bookReview.UserId),
                  It.Is<Guid>(a => a == _bookReview.BookId)),
              Times.Once);

            _bookRepositoryMock.Verify(
              x => x.CreateReviewAsync(It.Is<BookReview>(x => x.UserId == _bookReview.UserId && x.BookId == _bookReview.BookId)),
              Times.Never);

            _unitOfWorkMock.Verify(
              x => x.SaveChangesAsync(),
              Times.Never);
            Assert.Equal(ExpectedException.Message, ActualException.Message);

        }


    }
}
