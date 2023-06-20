using FluentAssertions;
using Libro.ApiTest.Responses;
using Libro.Domain.Entities;
using Libro.Domain.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Libro.ApiTest
{
    public class GetBookReviewsTest : IntegrationTest
    {
        private readonly User _patron;
        private readonly BookReview _bookReview;
        private readonly Book _book;
        public GetBookReviewsTest():base() {
            _patron = new User()
            {
                Id = Guid.NewGuid(),
                Email = "patronBookReview231@libro.com",
                UserName = "patronBookReview231",
                Password = "password"
            };

            _book = new Book()
            {
                Id = Guid.NewGuid(),
                Title = "Tit45le",
                Genre = "gengfdfre",
                IsAvailable = true,
                PublishedDate = DateTime.Now.AddYears(-2),

            };

            _bookReview = new BookReview()
            {
                UserId = _patron.Id,
                BookId = _book.Id,
                Rate = 3,
                Review = "goo3d"
            };


            _context.Users.Add(_patron);
            _context.Books.Add(_book);
            _context.BookReviews.Add(_bookReview);
            _context.SaveChanges();

        }

        [Fact]
        public async Task GetBookReviews()
        {

            //Arrange
           
            var bookId= _book.Id;

             //Act

             //401 Unauthrized           
             var unauthrizedResponse = await _client.GetAsync($"/Book/{bookId}/Reviews");

             //403 Forbidden
             await AuthenticateAsync();
             var forBiddenResponse = await _client.GetAsync($"/Book/{bookId}/Reviews");

             //404 Ok
             await AuthenticateAsync("patron");
             var notFoundResponse = await _client.GetAsync($"/Book/{Guid.NewGuid()}/Reviews");
             var objectnotFoundResponse = await notFoundResponse.Content.ReadFromJsonAsync<ErrorResponse>();

             //200 Ok

             var okResponse = await _client.GetAsync($"/Book/{bookId}/Reviews");
             var objectOkResponse = await okResponse.Content.ReadFromJsonAsync<BookReviewsResponse>();


             //Assert
             unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

             forBiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

             notFoundResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
             objectnotFoundResponse.Should().BeOfType<ErrorResponse>();

             okResponse.StatusCode.Should().Be(HttpStatusCode.OK);
             objectOkResponse.Pages.Should().Be(1);

             objectOkResponse.Reviews.Should().NotBeNullOrEmpty();
             objectOkResponse.Reviews.First().BookId.Should().Be(bookId);
             objectOkResponse.Reviews.First().UserId.Should().Be(_patron.Id);
             

        }
    }
}
