using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

            user.Id = Guid.NewGuid();
            user.Email = user.Email.ToLower();
            user.UserName = user.UserName?.ToLower();
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.PhoneNumber = user.PhoneNumber?.ToLower();
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task ExceptionIfUserExistsAsync(User User)
        {
            if (User.Email is not null)
                if (await _context.Users.AnyAsync(e => e.Email == User.Email.ToLower()))
                    throw new UserExistsException(nameof(User.Email));
            if (User.UserName is not null)
                if (await _context.Users.AnyAsync(predicate: e => e.UserName == User.UserName.ToLower()))
                    throw new UserExistsException(nameof(User.UserName));
            if (User.UserName is not null)
                if (await _context.Users.AnyAsync(e => e.PhoneNumber == User.PhoneNumber.ToLower()))
                    throw new UserExistsException(nameof(User.PhoneNumber));
        }

        public async Task<string> Authenticate(User user)
        {

            var roleIds = _context.UserRoles.Where(e => e.UserId == user.Id).Select(r => r.RoleId).ToList();
            var roles = _context.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Name).ToList();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:SecretForKey"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claimsForToken = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
                new Claim("Email", user.Email),
                new Claim("UserName", user.UserName),
                new Claim("Roles", JsonConvert.SerializeObject(roles),JsonClaimValueTypes.JsonArray)
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

        public async Task<User?> ValidateUserCredentialsAsync(string email, string password)
        {

            var user = await _context.Users.FirstOrDefaultAsync(e => e.Email == email.ToLower());
            if (user is null)
                return null;
            if (BCrypt.Net.BCrypt.Verify(password, user.Password))
                return user;
            return null;
        }



    }
}
