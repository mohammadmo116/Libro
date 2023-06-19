using FluentAssertions;
using Libro.Domain.Entities;
using Libro.Domain.Responses;
using Libro.Presentation.Dtos.Author;
using Libro.Presentation.Dtos.User;
using Mapster;
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
    public class AuthorControllerTest : IntegrationTest
    {
      
        public AuthorControllerTest() : base() {
            var authorDto = new CreateAuthorDto()
            {
                Name = "Author",
                DateOfBirth = DateTime.Now.AddYears(-25),

            };
            var author = authorDto.Adapt<Author>();
            author.Id= Guid.NewGuid();
            _context.Authors.Add(author);
            _context.SaveChanges();
        }
        [Fact]
        public async Task CreateAuthor()
        {

            //Arrange
            var author1 = new CreateAuthorDto()
            {
               Name="Author1",
               DateOfBirth=DateTime.Now.AddYears(-20),

            };
            var author2 = new CreateAuthorDto()
            {
                Name = "Author1",
                DateOfBirth = DateTime.Now.AddDays(1),

            };

            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.PostAsJsonAsync("/Author", author1);

            //403 forbidden
            await AuthenticateAsync("patron");
            var forbiddenResponse = await _client.PostAsJsonAsync("/Author", author1);

            //201 Created
            await AuthenticateAsync("librarian");
            var createdResponse = await _client.PostAsJsonAsync("/Author", author1);


            //400 UserExists
            var badRequentResponse = await _client.PostAsJsonAsync("/Author", author2);
            var objectBadRequentResponse = await badRequentResponse.Content.ReadFromJsonAsync<ErrorResponse>();


            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            createdResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            badRequentResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            objectBadRequentResponse.Should().BeOfType<ErrorResponse>();

        }
       
        [Fact]
        public async Task GetAuthor()
        {

            //Arrange
            var author=_context.Authors.First();
            var authorId = author.Id;

            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.GetAsync($"/Author/{authorId}");

            //403 Forbidden
            await AuthenticateAsync();
            var forBiddenResponse = await _client.GetAsync($"/Author/{authorId}");

            //200 Ok
            await AuthenticateAsync("patron");
            var okResponse = await _client.GetAsync($"/Author/{authorId}");

            //404 author notfound
            var notFoundResponse = await _client.GetAsync($"/Author/{Guid.NewGuid()}");
            var objectNotFoundResponse = await notFoundResponse.Content.ReadFromJsonAsync<ErrorResponse>();




            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forBiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            notFoundResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            objectNotFoundResponse.Should().BeOfType<ErrorResponse>();


        }
        [Fact]
        public async Task UpdateAuthor()
        {

            //Arrange
            var author = _context.Authors.First();
            var authorId = author.Id;

            var TestId = Guid.NewGuid();
            var updateAuthor = new AuthorDto()
            {
                Id = authorId,
                Name = "Author"
            };
            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.PutAsJsonAsync($"/Author/{authorId}", updateAuthor);

            //403 forbidden if the user not patron
            await AuthenticateAsync("patron");
            var forbiddenResponse = await _client.PutAsJsonAsync($"/Author/{authorId}", updateAuthor);


            await AuthenticateAsync("librarian");

            //200 Ok
            var okResponse = await _client.PutAsJsonAsync($"/Author/{authorId}", updateAuthor);

            //400 bad request DateOfBirth in the furute or name field is required      
            updateAuthor.DateOfBirth = DateTime.UtcNow.AddDays(10);
            var badRequestResponse = await _client.PutAsJsonAsync($"/Author/{authorId}", updateAuthor);
            var objectBadRequestResponse = await badRequestResponse.Content.ReadFromJsonAsync<ErrorResponse>();

            //400 bad request when id in route != id in the body   
            updateAuthor.DateOfBirth = DateTime.UtcNow.AddDays(1);
            var badRequestResponse2 = await _client.PutAsJsonAsync($"/Author/{TestId}", updateAuthor);
            var objectBadRequestResponse2 = await badRequestResponse2.Content.ReadFromJsonAsync<ErrorResponse>();

            //404 Author not found
            updateAuthor.Id=TestId;
            updateAuthor.DateOfBirth = DateTime.UtcNow.AddYears(-30);
            var notFoundResponse = await _client.PutAsJsonAsync($"/Author/{TestId}", updateAuthor);
            var objectNotFoundResponse = await notFoundResponse.Content.ReadFromJsonAsync<ErrorResponse>();


           


            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            notFoundResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            objectNotFoundResponse.Should().BeOfType<ErrorResponse>();

            badRequestResponse2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            objectBadRequestResponse2.Should().BeOfType<ErrorResponse>();

            badRequestResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            objectBadRequestResponse.Should().BeOfType<ErrorResponse>();

        }
        [Fact]
        public async Task DeleteLibrarian()
        {

            //Arrange
            var author = _context.Authors.First();
            var authorId = author.Id;

            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.DeleteAsync($"/Author/{authorId}");

            //403 forbidden if the user not patron
            await AuthenticateAsync("patron");
            var forbiddenResponse = await _client.DeleteAsync($"/Author/{authorId}");


            await AuthenticateAsync("librarian");
            //200 Ok
            var okResponse = await _client.DeleteAsync($"/Author/{authorId}");

            //404 Author or not found
            var notFoundResponse = await _client.DeleteAsync($"/Author/{Guid.NewGuid()}");
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
