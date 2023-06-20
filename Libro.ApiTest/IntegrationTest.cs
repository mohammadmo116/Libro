using Libro.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.Http.Headers;
using Libro.Presentation;
using System.Net.Http.Json;
using Libro.Presentation.Dtos.User;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.TestHost;
using Libro.Presentation.Dtos.Role;
using Libro.Domain.Entities;
using System.Net.Sockets;

namespace Libro.ApiTest
{
    public class IntegrationTest : IClassFixture<WebApplicationFactory<Program>>
    {
        protected readonly ApplicationDbContext? _context;
        protected readonly HttpClient _client;
        protected readonly User _user;
        protected readonly User _adminUser;
        protected readonly User _librarianUser;
        protected readonly User _librarianUser2;
        protected readonly User _patronUser;
        public IntegrationTest() {

                var appFactory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder => {
                    builder.UseEnvironment("IntegrationTesting");
                });

            var roles = new List<Role>() {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name="admin".ToLower(),
                },
                 new()
                {
                    Id = Guid.NewGuid(),
                    Name="librarian".ToLower(),
                },
                  new()
                {
                    Id = Guid.NewGuid(),
                    Name="patron".ToLower(),
                },

            };

            _user = new User()
            {
                Id = Guid.NewGuid(),
                Email = "user@Libro.com".ToLower(),
                UserName = "user".ToLower(),
                Password = BCrypt.Net.BCrypt.HashPassword("password"),
                PhoneNumber = "3112354".ToLower()
            };
            _adminUser = new User()
            {
                Id = Guid.NewGuid(),
                Email = "admin@Libro.com".ToLower(),
                UserName = "admin".ToLower(),
                Password = BCrypt.Net.BCrypt.HashPassword("password"),
                PhoneNumber = "112354".ToLower()
            };
            _adminUser.Roles.Add(roles.FirstOrDefault(a => a.Name == "admin"));

            _librarianUser = new User()
            {
                Id = Guid.NewGuid(),
                Email = "librarian@Libro.com".ToLower(),
                UserName = "librarian".ToLower(),
                Password = BCrypt.Net.BCrypt.HashPassword("password"),
                PhoneNumber = "1123524".ToLower()               
            };
            _librarianUser.Roles.Add(roles.FirstOrDefault(a => a.Name == "librarian"));

            _librarianUser2 = new User()
            {
                Id = Guid.NewGuid(),
                Email = "librarian2@Libro.com".ToLower(),
                UserName = "librarian2".ToLower(),
                Password = BCrypt.Net.BCrypt.HashPassword("password"),
                PhoneNumber = "11235247".ToLower()
            };
            _librarianUser2.Roles.Add(roles.FirstOrDefault(a => a.Name == "librarian"));


             _patronUser = new User()
            {
                Id = Guid.NewGuid(),
                Email = "patron@Libro.com".ToLower(),
                UserName = "Patron".ToLower(),
                Password = BCrypt.Net.BCrypt.HashPassword("password"),
                PhoneNumber = "11235334".ToLower()
            };
            _patronUser.Roles.Add(roles.FirstOrDefault(a => a.Name == "patron"));

         

            _context = appFactory.Services.GetRequiredService<ApplicationDbContext>();
            _context.Roles.AddRange(roles);
            _context.Users.Add(_adminUser);
            _context.Users.Add(_librarianUser);
            _context.Users.Add(_librarianUser2);
            _context.Users.Add(_patronUser);
            _context.Users.Add(_user);
             _context.SaveChanges();
            



            _client = appFactory.CreateClient();
           
        }
        
        protected async Task AuthenticateAsync(string? role="")
        {
            var token = await GetJwtAsync(role);           
           _client.DefaultRequestHeaders.Authorization =new AuthenticationHeaderValue("Bearer",token);

        }
       
       
        private async Task<string> GetJwtAsync(string? role)
        {
            HttpResponseMessage? response=new();
            if (string.IsNullOrEmpty(role))
            {
                response = await _client.PostAsJsonAsync("/Authentication/Login", new LoginUserDto()
                {
                    Email = "user@Libro.com".ToLower(),
                    Password = "password"
                });

            }
            if (role == "patron")
            {
 
                 response = await _client.PostAsJsonAsync("/Authentication/Login", new LoginUserDto()
                {
                    Email = "patron@Libro.com".ToLower(),
                    Password = "password"
                 });
            }
            if (role == "librarian")
            {

                 response = await _client.PostAsJsonAsync("/Authentication/Login", new LoginUserDto()
                {
                     Email = "librarian@Libro.com".ToLower(),
                     Password = "password"
                 });
            }
            if (role == "admin")
            {

                 response = await _client.PostAsJsonAsync("/Authentication/Login", new LoginUserDto()
                {
                     Email = "admin@Libro.com".ToLower(),
                     Password = "password"
                });
            }
            var JWTtoken = await response.Content.ReadAsStringAsync();
                return JWTtoken;
            
        }
    }
}

