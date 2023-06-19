using FluentAssertions;
using Libro.Domain.Responses;
using Libro.Presentation.Dtos.User;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;

namespace Libro.ApiTest
{
    public class AuthenticationControllerTest : IntegrationTest
    {

        public AuthenticationControllerTest() : base()
        {





        }
        [Fact]
        public async Task RegisterUser()
        {
            //Arrange       

            var user1 = new CreateUserDto()
            {
                UserName = "Test".ToLower(),
                Email = "Test@Test.com".ToLower(),
                Password = "string"

            };
            var user2 = new CreateUserDto()
            {
                UserName = "Test".ToLower(),
                Email = "Test@Test.com".ToLower(),
                Password = "string"

            };
            var user3 = new CreateUserDto()
            {
                UserName = "Test1".ToLower(),
                Email = "Test1@Test.com".ToLower(),
                Password = "string"

            };
            //Act

            //Ok
            var response1 = await _client.PostAsJsonAsync("/Authentication/Rgister", user1);
            var userResponse1 = await response1.Content.ReadFromJsonAsync<UserDtoWithId>();

            //400 UserExists
            var response2 = await _client.PostAsJsonAsync("/Authentication/Rgister", user2);
            var userResponse2 = await response2.Content.ReadFromJsonAsync<ErrorResponse>();

            //404 Role PatronNotFound
            var patronRoles =await _context.Roles.Where(a => a.Name.ToLower() == "patron").ToListAsync();
            _context.Roles.RemoveRange(patronRoles);
            await _context.SaveChangesAsync();
            var response3 = await _client.PostAsJsonAsync("/Authentication/Rgister", user3);
            var userResponse3 = await response3.Content.ReadFromJsonAsync<ErrorResponse>();

            //clean up
            await _context.Roles.AddAsync(patronRoles.First());
            _patronUser.Roles.Add(patronRoles.First());
            await _context.SaveChangesAsync();

            //Assert
            response1.StatusCode.Should().Be(HttpStatusCode.OK);
            userResponse1.Email.Should().Be("Test@Test.com".ToLower());

            response2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            userResponse2.Should().BeOfType<ErrorResponse>();

            response3.StatusCode.Should().Be(HttpStatusCode.NotFound);
            userResponse3.Should().BeOfType<ErrorResponse>();
        }
        [Fact]
        public async Task LoginUser()
        {
            //Arrange       
         
            var user1 = new LoginUserDto()
            {

                Email = "Test@Test.com",
                Password = "string"

            };
            var user2 = new LoginUserDto()
            {

                Email = "Test@Test.com",
                Password = "WrongPassword"

            };

            //Act

            //Ok
            var response1 = await _client.PostAsJsonAsync("/Authentication/Login", user1);
            var userResponse1 = await response1.Content.ReadAsStringAsync();

            //401 InvalidCredentials
            var response2 = await _client.PostAsJsonAsync("/Authentication/Login", user2);
          

            //Assert
            response1.StatusCode.Should().Be(HttpStatusCode.OK);
            userResponse1.Should().NotBeNullOrEmpty();
            userResponse1.Should().BeOfType<string>();

            response2.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        }

    }
}
