namespace Libro.Domain.Exceptions.UserExceptions
{
    public class UserOrRoleNotFoundException : Exception
    {
        public UserOrRoleNotFoundException()
            : base("UserOrRole Not_Found 404")
        {
        }
    }
}
