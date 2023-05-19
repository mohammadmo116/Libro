using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Infrastructure.Repositories
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthenticationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> RegisterUser(User user)
        {
           
            User User = new()
            {
                Email = user.Email,
                UserName = user.UserName ?? null,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash),
                PhoneNumber = user.PhoneNumber ?? null,
            };
            await _context.Users.AddAsync(User);
            await _context.SaveChangesAsync();
            return user;
        }


    }
}
