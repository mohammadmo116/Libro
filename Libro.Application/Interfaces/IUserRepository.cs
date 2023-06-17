using Libro.Domain.Entities;

namespace Libro.Application.Interfaces
{
    public interface IUserRepository
    {
        Task AssignRoleToUserAsync(UserRole userRole);
        Task<bool> UserHasTheAssignedRoleAsync(UserRole userRole);
        Task<bool> RoleOrUserNotFoundAsync(UserRole userRole);
        Task<User> RegisterUserAsync(User user);
        void UpdateUser(User user);
        void RemoveUser(User user);
        Task<bool> EmailIsUniqueAsync(string Email);
        Task<bool> EmailIsUniqueForUpdateAsync(Guid UserId, string Email);
        Task<bool> UserNameIsUniqueAsync(string UserName);
        Task<bool> UserNameIsUniqueForUpdateAsync(Guid UserId, string UserName);
        Task<bool> PhoneNumberIsUniqueAsync(string PhoneNumber);
        Task<bool> PhoneNumberIsUniqueForUpdateAsync(Guid UserId, string PhoneNumber);
        Task<User> GetUserAsync(Guid UserId);
        Task<User> GetUserWtithRolesAsync(Guid UserId);
        Task<(List<BookTransaction>, int)> GetBorrowingHistoryAsync(Guid UserId, int PageNumber, int Count);
        Task<List<User>> GetPatronsWithReservedBooksAsync();
        Task<List<User>> GetPatronsWithDueDatesAsync();
        Task<(List<Book>, int)> GetRecommendedBooksAsync(Guid UserId, int PageNumber, int Count);
    }
}