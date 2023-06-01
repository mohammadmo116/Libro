using Libro.Domain.Entities;

namespace Libro.Application.Interfaces
{
    public interface IUserRepository
    {
        Task AssignRoleToUserAsync(UserRole userRole);
        Task<bool> UserHasTheAssignedRoleAsync(UserRole userRole);
        Task<bool> RoleOrUserNotFoundAsync(UserRole userRole);
        Task<User> RegisterUserAsync(User user);
        Task<bool> EmailIsUniqueAsync(string Email);
        Task<bool> UserNameIsUniqueAsync(string UserName);
        Task<bool> PhoneNumberIsUniqueAsync(string PhoneNumber);
        Task<User> GetUserAsync(Guid UserId);
    }
}