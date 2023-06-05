using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Exceptions;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Libro.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserAsync(Guid UserId)
        {

            var user = await _context.Users
                .FindAsync(UserId);
            return user;


        }
        public async Task<User> GetUserWtithRolesAsync(Guid UserId)
        {

            var UserWithRoles= await _context.Users
                .Include(a => a.Roles)
                .FirstOrDefaultAsync(u=>u.Id==UserId);
            return UserWithRoles;


        }
      

        public async Task<User> RegisterUserAsync(User user)
        {

            user.Id = Guid.NewGuid();
            user.Email = user.Email.ToLower();
            user.UserName = user.UserName?.ToLower();
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.PhoneNumber = user.PhoneNumber?.ToLower();
            await _context.Users.AddAsync(user);
            return user;
        }

    
        public void UpdateUser(User user)
        {
           _context.Users.Update(user);
        }
        public void RemoveUser(User user)
        {
            _context.Users.Remove(user);
        }
        public async Task<bool> EmailIsUniqueAsync(string Email)
        {
            if (Email is not null)
                if (await _context.Users.AnyAsync(e => e.Email == Email.ToLower()))
                    return false;
            return true;
           
        }
        public async Task<bool> EmailIsUniqueForUpdateAsync(Guid UserId, string Email)
        {
            //TODO
            if (Email is not null)
                if (await _context.Users.Where(u => u.Id != UserId).AnyAsync(e => e.Email == Email.ToLower()))
                    return false;
            return true;

        }

        public async Task<bool> UserNameIsUniqueAsync(string UserName)
        {
            if (UserName is not null)
                if (await _context.Users.AnyAsync(e => e.UserName == UserName.ToLower()))
                    return false;
            return true;
         

        }
        public async Task<bool> UserNameIsUniqueForUpdateAsync(Guid UserId,string UserName)
        {
            if (UserName is not null)
                if (await _context.Users.Where(u => u.Id != UserId).AnyAsync(e => e.UserName == UserName.ToLower()))
                    return false;
            return true;


        }
        public async Task<bool> PhoneNumberIsUniqueAsync(string PhoneNumber)
        {
            if (PhoneNumber is not null)
                if (await _context.Users.AnyAsync(e => e.PhoneNumber == PhoneNumber.ToLower()))
                    return false;
            return true;
        

        }
        public async Task<bool> PhoneNumberIsUniqueForUpdateAsync(Guid UserId, string PhoneNumber)
        {
            if (PhoneNumber is not null)
                if (await _context.Users.Where(u => u.Id != UserId).AnyAsync(e => e.PhoneNumber == PhoneNumber.ToLower()))
                    return false;
            return true;


        }
        public async Task AssignRoleToUserAsync(UserRole userRole)
        {
            await _context.AddAsync(userRole);     
        }

        public async Task<bool> UserHasTheAssignedRoleAsync(UserRole userRole) 
        {

            return await _context.UserRoles.Where(e=>e.RoleId== userRole.RoleId).AnyAsync(e=>e.UserId==userRole.UserId);
        }

        public async Task<bool> RoleOrUserNotFoundAsync(UserRole userRole)
        {
           
            if (await _context.Users.FindAsync(userRole.UserId) is null)
                return true;
            if (await _context.Roles.FindAsync(userRole.RoleId) is null)
                return true;
            return false;
        }
        public async Task<List<BookTransaction>> GetBorrowingHistoryAsync(Guid UserId, int PageNumber, int Count)
        {


            return await _context.BookTransactions
                .Include(a => a.Book)
                .Where(a => a.UserId == UserId)
                .OrderByDescending(a => a.DueDate)
                .Skip(PageNumber * Count)
                .Take(Count)
                .ToListAsync();

        }
    }
}

