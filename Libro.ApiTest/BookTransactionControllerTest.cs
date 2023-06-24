using FluentAssertions;
using Libro.ApiTest.Responses;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Responses;
using Libro.Presentation.Dtos.BookTransaction;
using System.Net;
using System.Net.Http.Json;

namespace Libro.ApiTest
{
    public class BookTransactionControllerTest : IntegrationTest
    {
        public BookTransactionControllerTest() : base()
        {

        }
        [Fact]
        public async Task GetBookTransactiob()
        {

            //Arrange
            var book = new Book()
            {
                Id = Guid.NewGuid(),
                Title = "Title",
                Genre = "genre",
                IsAvailable = true,
                PublishedDate = DateTime.Now.AddYears(-3)

            };

            var bookTransaction = new BookTransaction()
            {
                Id = Guid.NewGuid(),
                BookId = book.Id,
                UserId = _patronUser.Id,
                Status = BookStatus.Borrowed,
                BorrowedDate = DateTime.UtcNow.AddDays(-1),
                DueDate = DateTime.UtcNow.AddDays(5),



            };

            _context.BookTransactions.Add(bookTransaction);
            _context.Books.Add(book);
            _context.SaveChanges();
            var bookTransactions = _context.BookTransactions.First();
            var TransactionId = bookTransactions.Id;
            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.GetAsync($"/Book/Transactions/{TransactionId}");

            //403 Forbidden
            await AuthenticateAsync();
            var forBiddenResponse = await _client.GetAsync($"/Book/Transactions/{TransactionId}");

            //200 Ok
            await AuthenticateAsync("patron");
            var okResponse = await _client.GetAsync($"/Book/Transactions/{TransactionId}");

            //404 book notfound
            var notFoundResponse = await _client.GetAsync($"/Book/Transactions/{Guid.NewGuid()}");
            var objectNotFoundResponse = await notFoundResponse.Content.ReadFromJsonAsync<ErrorResponse>();




            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forBiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);


            notFoundResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            objectNotFoundResponse.Should().BeOfType<ErrorResponse>();
        }

        [Fact]
        public async Task ReserveBook_AddBookTransaction()
        {

            //Arrange

            var book = new Book()
            {
                Id = Guid.NewGuid(),
                Title = "Title1",
                Genre = "genre1",
                IsAvailable = true,
                PublishedDate = DateTime.Now.AddYears(-3)

            };
            var book2 = new Book()
            {
                Id = Guid.NewGuid(),
                Title = "Title12",
                Genre = "genre12",
                IsAvailable = false,
                PublishedDate = DateTime.Now.AddYears(-4)

            };
            _context.Books.Add(book);
            _context.Books.Add(book2);
            _context.SaveChanges();

            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.PostAsJsonAsync($"/Book/{book.Id}/Reserve", new object());

            //403 Forbidden
            await AuthenticateAsync();
            var forBiddenResponse = await _client.PostAsJsonAsync($"/Book/{book.Id}/Reserve", new object());

            //200 Ok
            await AuthenticateAsync("patron");
            var okResponse = await _client.PostAsJsonAsync($"/Book/{book.Id}/Reserve", new object());

            //400 bad request book not available 
            var badRequestResponse = await _client.PostAsJsonAsync($"/Book/{book2.Id}/Reserve", new object());
            var objectbadRequestResponse = await badRequestResponse.Content.ReadFromJsonAsync<ErrorResponse>();

            //404 book notfound
            var notFoundResponse = await _client.PostAsJsonAsync($"/Book/{Guid.NewGuid()}/Reserve", new object());
            var objectNotFoundResponse = await notFoundResponse.Content.ReadFromJsonAsync<ErrorResponse>();




            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forBiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            badRequestResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            objectbadRequestResponse.Should().BeOfType<ErrorResponse>();

            notFoundResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            objectNotFoundResponse.Should().BeOfType<ErrorResponse>();
        }

        [Fact]
        public async Task BorrowBook_ModifyBookTransaction()
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
                Status = BookStatus.Reserved,



            };
            _context.Books.Add(book);
            _context.BookTransactions.Add(bookTransaction);
            _context.SaveChanges();


            //Act


            //401 Unauthrized           
            var unauthrizedResponse = await _client.PutAsJsonAsync($"/Book/Transactions/{bookTransaction.Id}/Borrow", new DueDateDto()
            {
                DueDate = DateTime.UtcNow.AddDays(14)
            });

            //403 Forbidden
            await AuthenticateAsync();
            var forBiddenResponse = await _client.PutAsJsonAsync($"/Book/Transactions/{bookTransaction.Id}/Borrow", new DueDateDto()
            {
                DueDate = DateTime.UtcNow.AddDays(14)
            });

            //200 Ok
            await AuthenticateAsync("librarian");
            var okResponse = await _client.PutAsJsonAsync($"/Book/Transactions/{bookTransaction.Id}/Borrow", new DueDateDto()
            {
                DueDate = DateTime.UtcNow.AddDays(14)
            });

            //400 bad request book already borrowed
            var badRequestResponse = await _client.PutAsJsonAsync($"/Book/Transactions/{bookTransaction.Id}/Borrow", new DueDateDto()
            {
                DueDate = DateTime.UtcNow.AddDays(14)
            });

            var objectbadRequestResponse = await badRequestResponse.Content.ReadFromJsonAsync<ErrorResponse>();

            //404 book notfound
            var notFoundResponse = await _client.PutAsJsonAsync($"/Book/Transactions/{Guid.NewGuid()}/Borrow", new DueDateDto()
            {
                DueDate = DateTime.UtcNow.AddDays(14)
            });

            var objectNotFoundResponse = await notFoundResponse.Content.ReadFromJsonAsync<ErrorResponse>();




            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forBiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            badRequestResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            objectbadRequestResponse.Should().BeOfType<ErrorResponse>();

            notFoundResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            objectNotFoundResponse.Should().BeOfType<ErrorResponse>();
        }

        [Fact]
        public async Task ReturnBook_ModifyBookTransaction()
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


            //Act


            //401 Unauthrized           
            var unauthrizedResponse = await _client.PutAsJsonAsync($"/Book/Transactions/{bookTransaction.Id}/Return", new object());

            //403 Forbidden
            await AuthenticateAsync();
            var forBiddenResponse = await _client.PutAsJsonAsync($"/Book/Transactions/{bookTransaction.Id}/Return", new object());

            //200 Ok
            await AuthenticateAsync("librarian");
            var okResponse = await _client.PutAsJsonAsync($"/Book/Transactions/{bookTransaction.Id}/Return", new object());

            //404 book notfound
            var notFoundResponse = await _client.PutAsJsonAsync($"/Book/Transactions/{Guid.NewGuid()}/Return", new object());

            var objectNotFoundResponse = await notFoundResponse.Content.ReadFromJsonAsync<ErrorResponse>();




            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forBiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            notFoundResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            objectNotFoundResponse.Should().BeOfType<ErrorResponse>();
        }

        [Fact]
        public async Task GetDueDatesTransactions()
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


            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.GetAsync($"/Book/Due-Date-Transactions?PageNumber=0&Count=5");

            //403 Forbidden
            await AuthenticateAsync();
            var forBiddenResponse = await _client.GetAsync($"/Book/Due-Date-Transactions?PageNumber=0&Count=5");

            //200 Ok
            await AuthenticateAsync("librarian");
            var okResponse = await _client.GetAsync($"/Book/Due-Date-Transactions?PageNumber=0&Count=5");
            var objectOkResponse = await okResponse.Content.ReadFromJsonAsync<DueDateBookTransactionsResponse>();


            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forBiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            objectOkResponse.Pages.Should().BeGreaterThan(0);
            objectOkResponse.Transactions.Should().NotBeNullOrEmpty();

        }

    }
}
