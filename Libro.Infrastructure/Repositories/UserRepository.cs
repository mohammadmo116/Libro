using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
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

            var UserWithRoles = await _context.Users
                .Include(a => a.Roles)
                .FirstOrDefaultAsync(u => u.Id == UserId);
            return UserWithRoles;


        }
        public async Task<List<Guid>> GetPatronIdsForReservedBooksAsync()
        {

            return await _context.Users
                 .Include(a => a.Roles)
                 .Include(a => a.BookTransactions)
                 .Where(u => u.Roles.Any(a => a.Name == "patron"))
                 .Where(u => u.BookTransactions.Any(a => a.Status == BookStatus.Reserved))
                 .Select(a => a.Id).ToListAsync();



        }
        public async Task<List<Guid>> GetPatronIdsForDueDatesAsync()
        {

            return await _context.Users
                 .Include(a => a.Roles)
                 .Include(a => a.BookTransactions)
                 .Where(u => u.Roles.Any(a => a.Name == "patron"))
                 .Where(u => u.BookTransactions.Any(a => a.Status == BookStatus.Borrowed))
                 .Select(a => a.Id).ToListAsync();



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
        public async Task<bool> UserNameIsUniqueForUpdateAsync(Guid UserId, string UserName)
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

            return await _context.UserRoles.Where(e => e.RoleId == userRole.RoleId).AnyAsync(e => e.UserId == userRole.UserId);
        }

        public async Task<bool> RoleOrUserNotFoundAsync(UserRole userRole)
        {

            if (await _context.Users.FindAsync(userRole.UserId) is null)
                return true;
            if (await _context.Roles.FindAsync(userRole.RoleId) is null)
                return true;
            return false;
        }
        public async Task<(List<BookTransaction>, int)> GetBorrowingHistoryAsync(Guid UserId, int PageNumber, int Count)
        {

            var bookTransactionsCount = await _context.BookTransactions
               .Include(a => a.Book)
               .Where(a => a.UserId == UserId)
               .CountAsync();

            var bookTransactions = await _context.BookTransactions
                .Include(a => a.Book)
                .Where(a => a.UserId == UserId)
                .OrderByDescending(a => a.DueDate)
                .Skip(PageNumber * Count)
                .Take(Count)
                .ToListAsync();

            var NumberOfPages = 1;
            if (bookTransactionsCount > 0)
                NumberOfPages = (int)Math.Ceiling((double)bookTransactionsCount / Count);

            return (bookTransactions, NumberOfPages);

        }
        public async Task<(List<Book>, int)> GetRecommendedBooksAsync(Guid UserId, int PageNumber, int Count)
        {
            var BookIds = _context.BookTransactions
              .Where(a => a.UserId == UserId)
              .Select(a => a.BookId);

            var FavirateAuthors = _context.AuthorBooks
                         .Where(a => BookIds.Contains(a.BookId))
                         .GroupBy(a => a.AuthorId)
                         .Select(a => new { AuthorId = a.Key, Count = a.Count() })
                         .OrderByDescending(a => a.Count)
                         .Take(2)
                         .Select(a => a.AuthorId);


            var RecommendedBooksByFavirateAuthors = _context.Books
                .Where(a => a.Authors.Any(b => FavirateAuthors.Contains(b.Id)));




            var FavirateGenres = _context.BookTransactions
                    .Where(a => a.UserId == UserId)
                    .GroupBy(a => a.Book.Genre)
                    .Where(d => d.Key != null)
                    .Select(a => new { Genre = a.Key, Count = a.Count() })
                    .OrderByDescending(a => a.Count)
                    .Take(2)
                    .Select(a => a.Genre);

            var RecommendedBooksByFavirateGenres = _context.Books
               .Where(a => FavirateGenres.Contains(a.Genre));

            var RecommendedBooksCount = await RecommendedBooksByFavirateAuthors
                                        .Union(RecommendedBooksByFavirateGenres)
                                        .CountAsync();

            var RecommendedBooks = await RecommendedBooksByFavirateAuthors
                                         .Union(RecommendedBooksByFavirateGenres)
                                            .Include(a => a.Authors)
                                             .Skip(PageNumber * Count)
                                             .Take(Count)
                                             .ToListAsync();



            var NumberOfPages = 1;
            if (RecommendedBooksCount > 0)
                NumberOfPages = (int)Math.Ceiling((double)RecommendedBooksCount / Count);

            return (RecommendedBooks, NumberOfPages);


        }
    }
}

