using FluentAssertions;
using Libro.ApiTest.Responses;
using System.Net.Http.Json;
using System.Net;
using Libro.Domain.Entities;
using Libro.Presentation.Dtos.BookReview;
using Libro.Domain.Responses;
using Libro.Domain.Enums;
using Xunit;

namespace Libro.ApiTest
{
    public class BookReviewControllerTest : IntegrationTest
    {
        private readonly Book _book;
        private readonly User _patron;
        private readonly BookReview _bookReview;

        public BookReviewControllerTest() : base()
        {

             _book = new Book()
            {
                Id = Guid.NewGuid(),
                Title = "Title",
                Genre = "genre",
                IsAvailable = true,
                PublishedDate = DateTime.Now.AddYears(-3),

            };

             _patron = new User()
            {
                Id = Guid.NewGuid(),
                Email = "patronBookReview21@libro.com",
                UserName = "patronBookReview21",
                Password = "password"
            };

             _bookReview = new BookReview()
            {
                UserId = _patron.Id,
                BookId = _book.Id,
                Rate = 5,
                Review = "good"
             };

            _context.Users.Add(_patron);   
            _context.Books.Add(_book);
            _context.BookReviews.Add(_bookReview);
            _context.SaveChanges();
        }

       

        [Fact]
        public async Task CreateBookReview()
        {

            //Arrange       

            BookTransaction bookTransaction = new()
                {
                    Id = Guid.NewGuid(),
                    UserId = _patronUser.Id,
                    BookId = _book.Id,
                    Status = BookStatus.Returned,
                    DueDate = DateTime.UtcNow.AddDays(1),
                    BorrowedDate = DateTime.UtcNow.AddDays(-10),

                };
            _context.BookTransactions.Add(bookTransaction);
            _context.SaveChanges();

            var bookReviewDto = new CreateBookReviewDto()
            {
               
                Rate = 4,
                Review = "good1"

            };

            var bookReviewDto2 = new CreateBookReviewDto()
            {

                Rate = 6,
                Review = "good14"

            };

            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.PostAsJsonAsync($"/Book/{_book.Id}/Review", bookReviewDto);

            //403 forbidden
            await AuthenticateAsync();
            var forbiddenResponse = await _client.PostAsJsonAsync($"/Book/{_book.Id}/Review", bookReviewDto);

            //200 Ok
            await AuthenticateAsync("patron");
            var okResponse = await _client.PostAsJsonAsync($"/Book/{_book.Id}/Review", bookReviewDto);

            //400 bad request - book is already reviewd
            var badRequentResponse = await _client.PostAsJsonAsync($"/Book/{_book.Id}/Review", bookReviewDto);
            var objectBadRequentResponse = await badRequentResponse.Content.ReadFromJsonAsync<ErrorResponse>();

            //400 bad request bad rate value //above 5 or bellow 0
            var badRequentResponse2 = await _client.PostAsJsonAsync($"/Book/{_book.Id}/Review", bookReviewDto2);
            var objectBadRequentResponse2 = await badRequentResponse2.Content.ReadFromJsonAsync<ErrorResponse>();

            //400 bad request book is not returned
            bookTransaction.Status = BookStatus.Borrowed;
            _context.BookTransactions.Update(bookTransaction);
            _context.SaveChanges();
            var badRequentResponse3 = await _client.PostAsJsonAsync($"/Book/{_book.Id}/Review", bookReviewDto);
            var objectBadRequentResponse3 = await badRequentResponse3.Content.ReadFromJsonAsync<ErrorResponse>();

            //404 Not Found when Book Not Found
            var notFoundResponse = await _client.PostAsJsonAsync($"/Book/{Guid.NewGuid()}/Review", bookReviewDto);
            var objectnotFoundResponse = await notFoundResponse.Content.ReadFromJsonAsync<ErrorResponse>();

            //Assert

            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            notFoundResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            objectnotFoundResponse.Should().BeOfType<ErrorResponse>();

            badRequentResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            objectBadRequentResponse.Should().BeOfType<ErrorResponse>();

            badRequentResponse2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            objectBadRequentResponse2.Should().BeOfType<ErrorResponse>();

            badRequentResponse3.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            objectBadRequentResponse3.Should().BeOfType<ErrorResponse>();

            
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
            await AuthenticateAsync("patron");
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
