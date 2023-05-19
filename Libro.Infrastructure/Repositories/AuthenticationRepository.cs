using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Infrastructure.Repositories
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public AuthenticationRepository(ApplicationDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<User> RegisterUserAsync(User user)
        {
            if(await EmailExists(user.Email))
                throw new UserExistsException("Email", user.Email);
            if(await UserNameExists(user.UserName))
                throw new UserExistsException("UserName", user.UserName);
            if (await PhoneNumberExists(user.PhoneNumber))
                throw new UserExistsException("PhoneNumber", user.PhoneNumber);

            User newUser = new()
            {
                
                Id = Guid.NewGuid(),
                Email = user.Email.ToLower(),
                UserName = user.UserName.ToLower() ?? null,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
                PhoneNumber = user.PhoneNumber ?? null,
            };
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            return newUser;
        }
        private async Task<bool> EmailExists(string Email) 
        {
             return await _context.Users.AnyAsync(e => e.Email == Email.ToLower());
                   
              
        }

        private async Task<bool> UserNameExists(string UserName)
        {
              return await _context.Users.AnyAsync(e => e.UserName == UserName.ToLower());
            
        }
        private async Task<bool> PhoneNumberExists(string PhoneNumber)
        {
             return await _context.Users.AnyAsync(e => e.PhoneNumber == PhoneNumber.ToLower());
          
        }
        public async Task<string> Authenticate(string Email, string Password)
        {
            var user = await ValidateUserCredentials(Email, Password);
            if (user is null)
                return null;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:SecretForKey"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claimsForToken = new List<Claim>
            {
                new Claim("sub", user.Id.ToString()),
                new Claim("email", user.Email),
                new Claim("username", user.UserName)
                //new Claim("roles", user.Roles.ToString())
            };
            var jwtSecurityToken = new JwtSecurityToken(_configuration["Authentication:Issuer"],
                 _configuration["Authentication:Audience"],
                 claimsForToken,
                 DateTime.UtcNow,
                 DateTime.UtcNow.AddHours(1),
                signingCredentials);
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return token;

        }

        private async Task<User?> ValidateUserCredentials(string email,string password)
        {
           var user = await _context.Users.FirstOrDefaultAsync(e=>e.Email==email.ToLower());
            if (user is null)
                return null;
            if (BCrypt.Net.BCrypt.Verify(password, user.Password))
                return user;
            return null;
        }
       
    }
}
