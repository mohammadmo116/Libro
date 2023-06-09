using Libro.Application.BookReviews.Commands;
using Libro.Application.BookReviews.Queries;
using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Assert = Xunit.Assert;

namespace Libro.Test.BookReviews
{
    public class GetBookReviewsQueryHandlerTest
    {

        private readonly Book _book;
        private readonly List<BookReview> _bookReviewsList;
        private readonly GetBookReviewsQuery _query;
        private readonly GetBookReviewsQueryHandler _handler;
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<ILogger<GetBookReviewsQueryHandler>> _loggerMock;

        public GetBookReviewsQueryHandlerTest()
        {


            _book = new()
            {
                Id = Guid.NewGuid(),
                Title = "title",
                IsAvailable = true,
                Genre = "genre"
            };

            _bookReviewsList = new()
            {
                
               new() {
                UserId = Guid.NewGuid(),
                BookId = _book.Id,
                Rate = 5,
                Review = "Greate" },
                 new(){ 
                UserId = Guid.NewGuid(),
                BookId = _book.Id,
                Rate = 3,
                Review = "good"},
                       new(){
                UserId = Guid.NewGuid(),
                BookId = _book.Id,
                Rate = 2,
                Review = "not bad"},

            };

            _bookRepositoryMock = new();
            _loggerMock = new();
            _query = new(_book.Id,1,1);
            _handler = new(
                _loggerMock.Object,
                _bookRepositoryMock.Object 
                );
        }

        [Fact]
        public async Task Handle_Should_ReturnBookReviews_WhenBookIsFound()
        {
            //Arrange

            _bookRepositoryMock.Setup(
             x => x.GetBookAsync(
                 It.IsAny<Guid>()))
               .ReturnsAsync(_book);

            _bookRepositoryMock.Setup(
              x => x.GetReviewsAsync(
                  It.IsAny<Guid>(),
                  It.IsAny<int>(),
                  It.IsAny<int>()))
                .ReturnsAsync((_bookReviewsList,1));

            //Act
            var result = await _handler.Handle(_query, default);

            //Assert  

            _bookRepositoryMock.Verify(
              x => x.GetBookAsync(
                  It.Is<Guid>(a => a == _book.Id)),
              Times.Once);

            _bookRepositoryMock.Verify(
              x => x.GetReviewsAsync(
                  It.Is<Guid>(a => a == _book.Id),
                  It.Is<int>(a => a == 1),
                  It.Is<int>(a => a == 1)),
              Times.Once);


            CollectionAssert.AreEqual(_bookReviewsList, result.Item1);
            Assert.Equal(1, result.Item2);
        }


        [Fact]
        public async Task Handle_Should_ThrowCustomNotFoundException_WhenBookIsNotFound()
        {

            //Arrange

            _bookRepositoryMock.Setup(
              x => x.GetBookAsync(
                  It.IsAny<Guid>()))
                .ReturnsAsync(() => null!);


            //Act
            async Task act() => await _handler.Handle(_query, default);
            CustomNotFoundException ActualException = await Assert.ThrowsAsync<CustomNotFoundException>(act);
            CustomNotFoundException ExpectedException = new("Book");

            //Assert
  

            _bookRepositoryMock.Verify(
              x => x.GetBookAsync(
                  It.Is<Guid>(a => a == _book.Id)),
              Times.Once);

            _bookRepositoryMock.Verify(
        x => x.GetReviewsAsync(
            It.Is<Guid>(a => a == _book.Id),
            It.Is<int>(a => a == 1),
            It.Is<int>(a => a == 1)),
        Times.Never);

            Assert.Equal(ExpectedException.Message, ActualException.Message);

        }
    }
}
