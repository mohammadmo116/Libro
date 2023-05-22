namespace Libro.Infrastructure.Authorization
{
    public interface IRoleService
    {
    Task<HashSet<string>> GetRolesAsync(Guid UserId);
    }
}
