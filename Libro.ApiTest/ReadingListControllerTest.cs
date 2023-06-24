using FluentAssertions;
using Libro.ApiTest.Responses;
using Libro.Domain.Entities;
using Libro.Domain.Responses;
using Libro.Presentation.Dtos.ReadingList;
using System.Net;
using System.Net.Http.Json;

namespace Libro.ApiTest
{
    public class ReadingListControllerTest : IntegrationTest
    {
        public ReadingListControllerTest() : base()
        {
            var readingList = new ReadingList()
            {
                Id = Guid.NewGuid(),
                Name = "Test435",
                Private = false,
                UserId = _patronUser.Id
            };
            var book = new Book()
            {
                Id = Guid.NewGuid(),
                Title = "Title7",
                Genre = "genre7",
                IsAvailable = true,
                PublishedDate = DateTime.Now.AddYears(-4),

            };

            readingList.Books.Add(book);
            _context.ReadingLists.Add(readingList);
            _context.SaveChanges();
        }
        [Fact]
        public async Task CreateReadingList()
        {

            //Arrange
            var readingList1 = new CreateReadingListDto()
            {
                Name = "Test1",
                Private = false

            };

            var readingList2 = new CreateReadingListDto()
            {
                Private = true
            };

            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.PostAsJsonAsync("/ReadingList", readingList1);

            //403 forbidden
            await AuthenticateAsync();
            var forbiddenResponse = await _client.PostAsJsonAsync("/ReadingList", readingList1);

            //201 Created
            await AuthenticateAsync("patron");
            var createdResponse = await _client.PostAsJsonAsync("/ReadingList", readingList1);
            var objectBadRequentRe2sponse2 = await createdResponse.Content.ReadFromJsonAsync<ErrorResponse>();

            //400 bad request required fields  
            var badRequentResponse = await _client.PostAsJsonAsync("/ReadingList", readingList2);
            var objectBadRequentResponse = await badRequentResponse.Content.ReadFromJsonAsync<ErrorResponse>();


            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            createdResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            badRequentResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            objectBadRequentResponse.Should().BeOfType<ErrorResponse>();

        }



        [Fact]
        public async Task GetReadingLists()
        {

            //Arrange
            var readingList = _context.ReadingLists.First();
            var readingListId = readingList.Id;

            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.GetAsync($"/ReadingList?PageNumber=0&Count=5");

            //403 Forbidden
            await AuthenticateAsync();
            var forBiddenResponse = await _client.GetAsync($"/ReadingList?PageNumber=0&Count=5");

            //200 Ok
            await AuthenticateAsync("patron");
            var okResponse = await _client.GetAsync($"/ReadingList?PageNumber=0&Count=5");
            var objectOkResponse = await okResponse.Content.ReadFromJsonAsync<ReadingListsResponse>();





            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forBiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            objectOkResponse.Pages.Should().Be(1);
            objectOkResponse.ReadingLists.Should().NotBeNullOrEmpty();
            objectOkResponse.ReadingLists.First().Id.Should().Be(readingListId);



        }
        [Fact]
        public async Task GetReadingListBooks()
        {

            //Arrange
            var readingList = _context.ReadingLists.First();
            var readingListId = readingList.Id;
            var book = _context.ReadingLists.First().Books.First();
            var bookId = book.Id;

            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.GetAsync($"/ReadingList/{readingListId}/Books?PageNumber=0&Count=5");

            //403 Forbidden
            await AuthenticateAsync();
            var forBiddenResponse = await _client.GetAsync($"/ReadingList/{readingListId}/Books?PageNumber=0&Count=5");

            //200 Ok
            await AuthenticateAsync("patron");
            var okResponse = await _client.GetAsync($"/ReadingList/{readingListId}/Books?PageNumber=0&Count=5");
            var objectOkResponse = await okResponse.Content.ReadFromJsonAsync<ReadingListWithBookResponse>();





            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forBiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            objectOkResponse.Pages.Should().Be(1);
            objectOkResponse.ReadingList.Id.Should().Be(readingListId);
            objectOkResponse.ReadingList.Books.Should().NotBeNullOrEmpty();


        }
        [Fact]
        public async Task UpdateReadingList()
        {

            //Arrange
            var readingList = _context.ReadingLists.First();
            var readingListId = readingList.Id;

            var TestId = Guid.NewGuid();
            var updatedReadingList = new UpdateReadingListDto()
            {
                Id = readingListId,
                Name = readingList.Name
            };

            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.PutAsJsonAsync($"/ReadingList/{readingListId}", updatedReadingList);

            //403 forbidden if the user not patron
            await AuthenticateAsync();
            var forbiddenResponse = await _client.PutAsJsonAsync($"/ReadingList/{readingListId}", updatedReadingList);


            await AuthenticateAsync("patron");
            //200 Ok
            var okResponse = await _client.PutAsJsonAsync($"/ReadingList/{readingListId}", updatedReadingList);

            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        }
        [Fact]
        public async Task DeleteReadingList()
        {

            //Arrange
            var readingList = _context.ReadingLists.First();
            var readingListId = readingList.Id;

            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.DeleteAsync($"/ReadingList/{readingListId}");

            //403 forbidden if the user not patron
            await AuthenticateAsync();
            var forbiddenResponse = await _client.DeleteAsync($"/ReadingList/{readingListId}");


            await AuthenticateAsync("patron");
            //200 Ok
            var okResponse = await _client.DeleteAsync($"/ReadingList/{readingListId}");

            //404 Book not found
            var notFoundResponse = await _client.DeleteAsync($"/ReadingList/{Guid.NewGuid()}");
            var objectNotFoundResponse = await notFoundResponse.Content.ReadFromJsonAsync<ErrorResponse>();

            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            notFoundResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            objectNotFoundResponse.Should().BeOfType<ErrorResponse>();

            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        }

        [Fact]
        public async Task RemoveBookFromReadingList()
        {

            //Arrange
            var readingList = new ReadingList()
            {
                Id = Guid.NewGuid(),
                Name = "Test4355",
                Private = false,
                UserId = _patronUser.Id
            };
            var book = new Book()
            {
                Id = Guid.NewGuid(),
                Title = "Title57",
                Genre = "genre57",
                IsAvailable = true,
                PublishedDate = DateTime.Now.AddYears(-4),

            };

            readingList.Books.Add(book);
            _context.ReadingLists.Add(readingList);
            _context.SaveChanges();
            var readingListId = readingList.Id;
            var bookId = book.Id;

            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.DeleteAsync($"/ReadingList/{readingListId}/Books/{bookId}");

            //403 forbidden if the user not patron
            await AuthenticateAsync();
            var forbiddenResponse = await _client.DeleteAsync($"/ReadingList/{readingListId}/Books/{bookId}");


            await AuthenticateAsync("patron");
            //200 Ok
            var okResponse = await _client.DeleteAsync($"/ReadingList/{readingListId}/Books/{bookId}");

            //404 ReadingList not found
            var notFoundResponse = await _client.DeleteAsync($"/ReadingList/{Guid.NewGuid()}/Books/{bookId}");
            var objectNotFoundResponse = await notFoundResponse.Content.ReadFromJsonAsync<ErrorResponse>();

            //404 Book not found
            var notFoundResponse1 = await _client.DeleteAsync($"/ReadingList/{readingListId}/Books/{Guid.NewGuid()}");
            var objectNotFoundResponse1 = await notFoundResponse1.Content.ReadFromJsonAsync<ErrorResponse>();

            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            notFoundResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            objectNotFoundResponse.Should().BeOfType<ErrorResponse>();

            notFoundResponse1.StatusCode.Should().Be(HttpStatusCode.NotFound);
            objectNotFoundResponse1.Should().BeOfType<ErrorResponse>();

            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        }

        [Fact]
        public async Task AddBookFromReadingList()
        {

            //Arrange
            var readingList = new ReadingList()
            {
                Id = Guid.NewGuid(),
                Name = "Test4355",
                Private = false,
                UserId = _patronUser.Id
            };
            var book = new Book()
            {
                Id = Guid.NewGuid(),
                Title = "Title57",
                Genre = "genre57",
                IsAvailable = true,
                PublishedDate = DateTime.Now.AddYears(-4),

            };
            _context.Books.Add(book);
            _context.ReadingLists.Add(readingList);
            _context.SaveChanges();
            var readingListId = readingList.Id;
            var bookId = book.Id;

            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.PostAsJsonAsync($"/ReadingList/{readingListId}/Books/{bookId}", new object());

            //403 forbidden if the user not patron
            await AuthenticateAsync();
            var forbiddenResponse = await _client.PostAsJsonAsync($"/ReadingList/{readingListId}/Books/{bookId}", new object());


            await AuthenticateAsync("patron");
            //200 Ok
            var okResponse = await _client.PostAsJsonAsync($"/ReadingList/{readingListId}/Books/{bookId}", new object());

            //409 Conflict when ReadingList already has The added Book
            var conflictResponse = await _client.PostAsJsonAsync($"/ReadingList/{readingListId}/Books/{bookId}", new object());

            //404 ReadingList not found
            var notFoundResponse = await _client.PostAsJsonAsync($"/ReadingList/{Guid.NewGuid()}/Books/{bookId}", new object());
            var objectNotFoundResponse = await notFoundResponse.Content.ReadFromJsonAsync<ErrorResponse>();

            //404 Book not found
            var notFoundResponse1 = await _client.PostAsJsonAsync($"/ReadingList/{readingListId}/Books/{Guid.NewGuid()}", new object());
            var objectNotFoundResponse1 = await notFoundResponse1.Content.ReadFromJsonAsync<ErrorResponse>();

            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            notFoundResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            objectNotFoundResponse.Should().BeOfType<ErrorResponse>();

            notFoundResponse1.StatusCode.Should().Be(HttpStatusCode.NotFound);
            objectNotFoundResponse1.Should().BeOfType<ErrorResponse>();

            conflictResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);

            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        }
    }
}
