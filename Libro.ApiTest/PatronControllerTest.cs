using FluentAssertions;
using Libro.ApiTest.Responses;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Responses;
using Libro.Presentation.Dtos.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Libro.ApiTest
{
    public class PatronControllerTest : IntegrationTest
    {
        [Theory]
        [InlineData("librarian")]
        [InlineData("admin")]
        public async Task GetPatron(string role)
        {

            //Arrange
            var patronId = _patronUser.Id;

            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.GetAsync($"/Patron/{patronId}");

            //403 forbidden if the user not patron
            await AuthenticateAsync("patron");
            var forbiddenResponse = await _client.GetAsync($"/Patron/{patronId}");


            await AuthenticateAsync(role);
            //403 forbidden if the managed user is not patron or not found
            var forbiddenResponse2 = await _client.GetAsync($"/Patron/{Guid.NewGuid()}");

            //200 Ok
            var okResponse = await _client.GetAsync($"/Patron/{patronId}");
            var objectOkResponse = await okResponse.Content.ReadFromJsonAsync<UserDto>();



            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            forbiddenResponse2.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            objectOkResponse.Email.Should().Be(_patronUser.Email);

        }
        [Theory]
        [InlineData("librarian")]
        [InlineData("admin")]
        public async Task UpdatePatron(string role)
        {

            //Arrange
            var TestId = Guid.NewGuid();
            var patronId = _patronUser.Id;
            var updateuser = new UpdateUserDto()
            {
                Id = patronId,
            };
            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.PutAsJsonAsync($"/Patron/{patronId}", updateuser);

            //403 forbidden if the user not patron
            await AuthenticateAsync("patron");
            var forbiddenResponse = await _client.PutAsJsonAsync($"/Patron/{patronId}", updateuser);

            //200 Ok
            await AuthenticateAsync(role);
            var okResponse = await _client.PutAsJsonAsync($"/Patron/{patronId}", updateuser);

            //400 UserExists         
            updateuser.UserName = "librarian2";
            var badRequestResponse = await _client.PutAsJsonAsync($"/Patron/{patronId}", updateuser);
            var objectBadRequestResponse = await badRequestResponse.Content.ReadFromJsonAsync<ErrorResponse>();


            //403 forbidden if the managed user is not patron or not found
            updateuser.Id = TestId;
            var forbiddenResponse2 = await _client.PutAsJsonAsync($"/Patron/{TestId}", updateuser);

            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            forbiddenResponse2.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            badRequestResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            objectBadRequestResponse.Should().BeOfType<ErrorResponse>();

        }

        [Fact]
        public async Task GetPatronBorrowingHistory()
        {

            //Arrange
                 var book = new Book()
              {
                  Id = Guid.NewGuid(),
                  Title = "Title3",
                  Genre = "genre3",
                  IsAvailable = true,
                  PublishedDate = DateTime.Now.AddYears(-3)

              };

              var bookTransaction = new BookTransaction()
              {
                  Id = Guid.NewGuid(),
                  BookId = book.Id,
                  UserId = _patronUser.Id,
                  Status = BookStatus.Borrowed,
                  BorrowedDate = DateTime.UtcNow,
                  DueDate = DateTime.UtcNow.AddDays(14)


              };
              _context.Books.Add(book);
              _context.BookTransactions.Add(bookTransaction);
              _context.SaveChanges();

            var patronId = _patronUser.Id;
            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.GetAsync($"/Patron/{patronId}/Borrowing-History?PageNumber=0&Count=5");

            //403 Forbidden
            await AuthenticateAsync();
            var forBiddenResponse = await _client.GetAsync($"/Patron/{patronId}/Borrowing-History?PageNumber=0&Count=5");
            
            await AuthenticateAsync("librarian");
            var forBiddenResponse2 = await _client.GetAsync($"/Patron/{Guid.NewGuid()}/Borrowing-History?PageNumber=0&Count=5");

            //200 Ok
            var okResponse = await _client.GetAsync($"/Patron/{patronId}/Borrowing-History?PageNumber=0&Count=5");
            var objectOkResponse = await okResponse.Content.ReadFromJsonAsync<DueDateBookTransactionsResponse>();


            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forBiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            forBiddenResponse2.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            objectOkResponse.Pages.Should().Be(1);
            objectOkResponse.Transactions.Should().NotBeNullOrEmpty();
            objectOkResponse.Transactions.First().Id.Should().Be(bookTransaction.Id);

        }
        [Fact]
        public async Task GetPatronRecommendedBooks()
        {

            //Arrange
            var book = new Book()
            {
                Id = Guid.NewGuid(),
                Title = "Title3",
                Genre = "genre3",
                IsAvailable = true,
                PublishedDate = DateTime.Now.AddYears(-3)

            };

            var bookTransaction = new BookTransaction()
            {
                Id = Guid.NewGuid(),
                BookId = book.Id,
                UserId = _patronUser.Id,
                Status = BookStatus.Returned,
                BorrowedDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(1)


            };
            
            _context.Books.Add(book);
            _context.BookTransactions.Add(bookTransaction);
            _context.SaveChanges();

            var patronId = _patronUser.Id;
            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.GetAsync($"/Patron/RecommendedBooks?PageNumber=0&Count=5");

            //403 Forbidden
            await AuthenticateAsync();
            var forBiddenResponse = await _client.GetAsync($"/Patron/RecommendedBooks?PageNumber=0&Count=5");

            //200 Ok
            await AuthenticateAsync("patron");
            var okResponse = await _client.GetAsync($"/Patron/RecommendedBooks?PageNumber=0&Count=5");
            var objectOkResponse = await okResponse.Content.ReadFromJsonAsync<RecommendedBooksResponse>();


            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forBiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            objectOkResponse.Pages.Should().Be(1);
            objectOkResponse.RecommendedBooks.Should().NotBeNullOrEmpty();
            objectOkResponse.RecommendedBooks.Where(a => a.Id == book.Id).Should().NotBeNullOrEmpty();

        }
    }
}
