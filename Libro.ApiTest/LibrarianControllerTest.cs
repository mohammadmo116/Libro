using Libro.Domain.Responses;
using Libro.Presentation.Dtos.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Libro.ApiTest
{
    public class LibrarianControllerTest : IntegrationTest
    {
        public LibrarianControllerTest() : base()
        {
        }
        [Fact]
        public async Task CreateLibrarian()
        {

            //Arrange
            var librarian1 = new CreateUserDto()
            {
                UserName = "Test2".ToLower(),
                Email = "Test2@Test.com".ToLower(),
                Password = "string"

            };
            var librarian2 = new CreateUserDto()
            {
                UserName = "Test2".ToLower(),
                Email = "Test2@Test.com".ToLower(),
                Password = "string"

            };
            var librarian3 = new CreateUserDto()
            {
                UserName = "Test3".ToLower(),
                Email = "Test3@Test.com".ToLower(),
                Password = "string"

            };
            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.PostAsJsonAsync("/Librarian", librarian1);

            //403 forbidden
            await AuthenticateAsync("patron");
            var forbiddenResponse = await _client.PostAsJsonAsync("/Librarian", librarian1);

            //201 Created
            await AuthenticateAsync("admin");
            var createdResponse = await _client.PostAsJsonAsync("/Librarian", librarian1);


            //400 UserExists
            var badRequentResponse = await _client.PostAsJsonAsync("/Librarian", librarian2);
            var objectBadRequentResponse = await badRequentResponse.Content.ReadFromJsonAsync<ErrorResponse>();

            //404 Role librarianRoleNotFound
            var librarianRole = await _context.Roles.Where(a => a.Name.ToLower() == "librarian".ToLower()).ToListAsync();
            _context.Roles.RemoveRange(librarianRole);
            await _context.SaveChangesAsync();

            var notFoundResponse = await _client.PostAsJsonAsync("/Librarian", librarian3);
            var objectNotFoundResponse = await notFoundResponse.Content.ReadFromJsonAsync<ErrorResponse>();

            //clean up
            await _context.Roles.AddAsync(librarianRole.First());
            _librarianUser.Roles.Add(librarianRole.First());
            await _context.SaveChangesAsync();

            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            createdResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            badRequentResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            objectBadRequentResponse.Should().BeOfType<ErrorResponse>();

            notFoundResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            objectNotFoundResponse.Should().BeOfType<ErrorResponse>();


        }
        [Fact]
        public async Task GetLibrarian()
        {

            //Arrange
            var librarianId=_librarianUser.Id;

            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.GetAsync($"/Librarian/{librarianId}");

            //403 forbidden if the user not patron
            await AuthenticateAsync("patron");
            var forbiddenResponse = await _client.GetAsync($"/Librarian/{librarianId}");


            await AuthenticateAsync("admin");
            //403 forbidden if the managed user is not Librarian or not found
            var forbiddenResponse2 = await _client.GetAsync($"/Librarian/{Guid.NewGuid()}");

            //200 Ok
            var okResponse = await _client.GetAsync($"/Librarian/{librarianId}");

         
           
            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            forbiddenResponse2.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);


        }
        [Fact]
        public async Task UpdateLibrarian()
        {

            //Arrange
            var TestId = Guid.NewGuid();
            var librarianId = _librarianUser.Id;
            var updateuser = new UpdateUserDto()
            {
                Id = librarianId,
            };
            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.PutAsJsonAsync($"/Librarian/{librarianId}", updateuser);

            //403 forbidden if the user not patron
            await AuthenticateAsync("patron");
            var forbiddenResponse = await _client.PutAsJsonAsync($"/Librarian/{librarianId}", updateuser);


            await AuthenticateAsync("admin");

            //200 Ok
            var okResponse = await _client.PutAsJsonAsync($"/Librarian/{librarianId}", updateuser);

            //400 UserExists         
            updateuser.UserName = "librarian2";
            var badRequestResponse = await _client.PutAsJsonAsync($"/Librarian/{librarianId}", updateuser);
            var objectBadRequestResponse = await badRequestResponse.Content.ReadFromJsonAsync<ErrorResponse>();


            //403 forbidden if the managed user is not Librarian or not found
            updateuser.Id = TestId;
            var forbiddenResponse2 = await _client.PutAsJsonAsync($"/Librarian/{TestId}", updateuser);

           



            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            forbiddenResponse2.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            badRequestResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            objectBadRequestResponse.Should().BeOfType<ErrorResponse>();

        }
        [Fact]
        public async Task DeleteLibrarian()
        {

            //Arrange
            var librarianId = _librarianUser2.Id;

            //Act

            //401 Unauthrized           
            var unauthrizedResponse = await _client.DeleteAsync($"/Librarian/{librarianId}");

            //403 forbidden if the user not patron
            await AuthenticateAsync("patron");
            var forbiddenResponse = await _client.DeleteAsync($"/Librarian/{librarianId}");


            await AuthenticateAsync("admin");

            //200 Ok
            var okResponse = await _client.DeleteAsync($"/Librarian/{librarianId}");

            //403 forbidden if the managed user is not Librarian or not found
            var forbiddenResponse2 = await _client.DeleteAsync($"/Librarian/{Guid.NewGuid()}");

         



            //Assert
            unauthrizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            forbiddenResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            forbiddenResponse2.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            okResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        }
    }
}
