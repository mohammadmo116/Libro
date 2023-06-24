using FluentAssertions;
using Libro.ApiTest.Responses;
using Libro.Domain.Entities;
using Libro.Domain.Responses;
using Libro.Presentation.Dtos.Book;
using Mapster;
using System.Net;
using System.Net.Http.Json;

namespace Libro.ApiTest
{
    public class BookControllerTest : IntegrationTest
    {
        public BookControllerTest() : base()
        {
            var bookDto = new CreateBookDto()
            {
                Title = "Title",
                Genre = "genre",
                IsAvailable = true,
                PublishedDate = DateTime.Now.AddYears(-3),

            };
            var book = bookDto.Adapt<Book>();
            book.Id = Guid.NewGuid();
            _context.Books.Add(book);
            _context.SaveChanges();
        }
        [Fact]
        public async Task CreateBook()
        {

            //Arrange
            var book1 = new CreateBookDto()
            {
                Title = "Title1",
                Genre = "genre1",
                IsAvailable = true,
                PublishedDate = DateTime.Now.AddYears(-4),

            };

            var book2 = new CreateBookDto()
            {
                Title = "Title2",
                Genre = "genre2",
                IsAvailable = true,
                PublishedDate = DateTime.Now.AddYears(5),

            };
            var book3 = new CreateBookDto()
            {
                Genre = "genre2",
                PublishedDate = DateTime.Now.AddYears(-5),

            };


            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.PostAsJsonAsync("/Book", book1);

            //403 forbidden
            await AuthenticateAsync("patron");
            var forbiddenResponse = await _client.PostAsJsonAsync("/Book", book1);

            //201 Created
            await AuthenticateAsync("librarian");
            var createdResponse = await _client.PostAsJsonAsync("/Book", book1);


            //400 bad request PublishedDate in the future  
            var badRequentResponse = await _client.PostAsJsonAsync("/Book", book2);
            var objectBadRequentResponse = await badRequentResponse.Content.ReadFromJsonAsync<ErrorResponse>();

            //400 bad request required fields  
            var badRequentResponse2 = await _client.PostAsJsonAsync("/Book", book3);
            var objectBadRequentResponse2 = await badRequentResponse2.Content.ReadFromJsonAsync<ErrorResponse>();


            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            createdResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            badRequentResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            objectBadRequentResponse.Should().BeOfType<ErrorResponse>();

            badRequentResponse2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            objectBadRequentResponse2.Should().BeOfType<ErrorResponse>();

        }

        [Fact]
        public async Task GetBook()
        {

            //Arrange
            var book = _context.Books.First();
            var bookId = book.Id;

            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.GetAsync($"/Book/{bookId}");

            //403 Forbidden
            await AuthenticateAsync();
            var forBiddenResponse = await _client.GetAsync($"/Book/{bookId}");

            //200 Ok
            await AuthenticateAsync("patron");
            var okResponse = await _client.GetAsync($"/Book/{bookId}");

            //404 book notfound
            var notFoundResponse = await _client.GetAsync($"/Book/{Guid.NewGuid()}");
            var objectNotFoundResponse = await notFoundResponse.Content.ReadFromJsonAsync<ErrorResponse>();




            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forBiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            notFoundResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            objectNotFoundResponse.Should().BeOfType<ErrorResponse>();


        }
        [Fact]
        public async Task GetBooks()
        {

            //Arrange
            var book = _context.Books.First();
            var bookId = book.Id;

            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.GetAsync($"/Book");

            //403 Forbidden
            await AuthenticateAsync();
            var forBiddenResponse = await _client.GetAsync($"/Book");

            //200 Ok
            await AuthenticateAsync("patron");
            var okResponse = await _client.GetAsync($"/Book");
            var objectOkResponse = await okResponse.Content.ReadFromJsonAsync<BookResponse>();


            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forBiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            objectOkResponse.Pages.Should().BeGreaterThan(0);
            objectOkResponse.books.Should().NotBeNullOrEmpty();
            objectOkResponse.books.First().Id.Should().Be(bookId);

        }
        [Fact]
        public async Task UpdateBook()
        {

            //Arrange
            var book = _context.Books.First();
            var bookId = book.Id;

            var TestId = Guid.NewGuid();
            var updateBook = new UpdateBookDto()
            {
                Id = bookId,
                Title = "title",
                PublishedDate = DateTime.UtcNow.AddYears(-3),

            };
            var updateBook2 = new UpdateBookDto()
            {
                Id = bookId,


            };
            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.PutAsJsonAsync($"/Book/{bookId}", updateBook);

            //403 forbidden if the user not patron
            await AuthenticateAsync("patron");
            var forbiddenResponse = await _client.PutAsJsonAsync($"/Book/{bookId}", updateBook);


            await AuthenticateAsync("librarian");

            //200 Ok
            var okResponse = await _client.PutAsJsonAsync($"/Book/{bookId}", updateBook);

            //400 bad request PublishedDate in the furute   
            updateBook.PublishedDate = DateTime.UtcNow.AddDays(10);
            var badRequestResponse = await _client.PutAsJsonAsync($"/Book/{bookId}", updateBook);
            var objectBadRequestResponse = await badRequestResponse.Content.ReadFromJsonAsync<ErrorResponse>();

            //400 bad request required fields      
            var badRequestResponse3 = await _client.PutAsJsonAsync($"/Book/{bookId}", updateBook2);
            var objectBadRequestResponse3 = await badRequestResponse3.Content.ReadFromJsonAsync<ErrorResponse>();


            //400 bad request when id in route != id in the body   
            updateBook.PublishedDate = DateTime.UtcNow.AddDays(1);
            var badRequestResponse2 = await _client.PutAsJsonAsync($"/Book/{TestId}", updateBook);
            var objectBadRequestResponse2 = await badRequestResponse2.Content.ReadFromJsonAsync<ErrorResponse>();

            //404 Author not found
            updateBook.Id = TestId;
            updateBook.PublishedDate = DateTime.UtcNow.AddYears(-30);
            var notFoundResponse = await _client.PutAsJsonAsync($"/Book/{TestId}", updateBook);
            var objectNotFoundResponse = await notFoundResponse.Content.ReadFromJsonAsync<ErrorResponse>();





            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            notFoundResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            objectNotFoundResponse.Should().BeOfType<ErrorResponse>();

            badRequestResponse3.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            objectBadRequestResponse3.Should().BeOfType<ErrorResponse>();

            badRequestResponse2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            objectBadRequestResponse2.Should().BeOfType<ErrorResponse>();

            badRequestResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            objectBadRequestResponse.Should().BeOfType<ErrorResponse>();

        }
        [Fact]
        public async Task DeleteBook()
        {

            //Arrange
            var book = _context.Books.First();
            var bookId = book.Id;

            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.DeleteAsync($"/Book/{bookId}");

            //403 forbidden if the user not patron
            await AuthenticateAsync("patron");
            var forbiddenResponse = await _client.DeleteAsync($"/Book/{bookId}");


            await AuthenticateAsync("librarian");
            //200 Ok
            var okResponse = await _client.DeleteAsync($"/Book/{bookId}");

            //404 Book not found
            var notFoundResponse = await _client.DeleteAsync($"/Book/{Guid.NewGuid()}");
            var objectNotFoundResponse = await notFoundResponse.Content.ReadFromJsonAsync<ErrorResponse>();

            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            notFoundResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            objectNotFoundResponse.Should().BeOfType<ErrorResponse>();

            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        }

    }
}
