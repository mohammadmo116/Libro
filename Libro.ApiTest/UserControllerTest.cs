using FluentAssertions;
using Libro.ApiTest.Responses;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Responses;
using Libro.Presentation.Dtos.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Libro.Presentation.Dtos.Book;

namespace Libro.ApiTest
{
    public class UserControllerTest : IntegrationTest
    {
        [Fact]
        public async Task GetUser()
        {

            //Arrange


            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.GetAsync($"/User");

            //403 forbidden if the user not patron
            await AuthenticateAsync();
            var forbiddenResponse = await _client.GetAsync($"/User");

            //200 Ok
            await AuthenticateAsync("patron");
            var okResponse = await _client.GetAsync($"/User");
            var objectOkResponse = await okResponse.Content.ReadFromJsonAsync<UserDto>();



            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            objectOkResponse.Email.Should().Be(_patronUser.Email);


        }


        [Fact]
        public async Task UpdateUser()
        {
            
            //Arrange
            var patronId = _patronUser.Id;
            var updateuser = new UpdateUserDto()
            {
                Id = patronId,
            };
            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.PutAsJsonAsync($"/User", updateuser);

            //403 forbidden if the user not patron
            await AuthenticateAsync();
            var forbiddenResponse = await _client.PutAsJsonAsync($"/User", updateuser);

            //200 Ok
            await AuthenticateAsync("patron");
            var okResponse = await _client.PutAsJsonAsync($"/User", updateuser);

            //400 UserExists         
            updateuser.UserName = "librarian2";
            var badRequestResponse = await _client.PutAsJsonAsync($"/User", updateuser);
            var objectBadRequestResponse = await badRequestResponse.Content.ReadFromJsonAsync<ErrorResponse>();


            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            badRequestResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            objectBadRequestResponse.Should().BeOfType<ErrorResponse>();
            
        }

       

        [Fact]
        public async Task GetUserBorrowingHistory()
        {

            //Arrange
            var book = new Book()
            {
                Id = Guid.NewGuid(),
                Title = "Title43",
                Genre = "genre34",
                IsAvailable = true,
                PublishedDate = DateTime.Now.AddYears(-4)

            };

            var bookTransaction = new BookTransaction()
            {
                Id = Guid.NewGuid(),
                BookId = book.Id,
                UserId = _patronUser.Id,
                Status = BookStatus.Borrowed,


            };
            _context.Books.Add(book);
            _context.BookTransactions.Add(bookTransaction);
            _context.SaveChanges();
           
            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.GetAsync($"/User/Borrowing-History?PageNumber=0&Count=5");

            //403 Forbidden
            await AuthenticateAsync();
            var forBiddenResponse = await _client.GetAsync($"/User/Borrowing-History?PageNumber=0&Count=5");

            await AuthenticateAsync("patron");
            //200 Ok
            var okResponse = await _client.GetAsync($"/User/Borrowing-History?PageNumber=0&Count=5");
            var objectOkResponse = await okResponse.Content.ReadFromJsonAsync<DueDateBookTransactionsResponse>();


            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forBiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            objectOkResponse.Pages.Should().Be(1);
            objectOkResponse.Transactions.Should().NotBeNullOrEmpty();
            objectOkResponse.Transactions.First().Id.Should().Be(bookTransaction.Id);

        }
        [Fact]
        public async Task AssignRoleToUser()
        {

            //Arrange
            var userr = new User()
            {
                Id = Guid.NewGuid(),
                Email = "user5@Libro.com".ToLower(),
                UserName = "user5".ToLower(),
                Password = BCrypt.Net.BCrypt.HashPassword("password"),
                PhoneNumber = "3112544".ToLower()
            };
            _context.Users.Add(userr);
            var role =_context.Roles.FirstOrDefault(a => a.Name == "patron");
            var userId = userr.Id;
            var roleId= role.Id;
            _context.SaveChanges();

            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.PostAsJsonAsync($"/User/{userId}/Role/{roleId}", new object());

            //403 forbidden
            await AuthenticateAsync("patron");
            var forbiddenResponse = await _client.PostAsJsonAsync($"/User/{userId}/Role/{roleId}", new object());

            //200 Ok
            await AuthenticateAsync("admin");
            var okResponse = await _client.PostAsJsonAsync($"/User/{userId}/Role/{roleId}", new object());


            //404 Not Found when user not found
            var notFoundResponse = await _client.PostAsJsonAsync($"/User/{Guid.NewGuid()}/Role/{roleId}", new object());

            //404 Not Found when role not found
            var notFoundResponse2 = await _client.PostAsJsonAsync($"/User/{userId}/Role/{Guid.NewGuid()}", new object());

            //409 Conflict when User already has The Assigned Role
            var conflictResponse = await _client.PostAsJsonAsync($"/User/{userId}/Role/{roleId}", new object());

            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            notFoundResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

            notFoundResponse2.StatusCode.Should().Be(HttpStatusCode.NotFound);

            conflictResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }


    }
}
