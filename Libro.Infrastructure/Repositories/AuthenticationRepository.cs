using Libro.Application.Interfaces;
using Libro.Domain.Entities;
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

        public async Task<string> Authenticate(User user)
        {

            var roleIds = await _context.UserRoles.Where(e => e.UserId == user.Id).Select(r => r.RoleId).ToListAsync();
            var roles = await _context.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Name).ToListAsync();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:SecretForKey"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claimsForToken = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
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
