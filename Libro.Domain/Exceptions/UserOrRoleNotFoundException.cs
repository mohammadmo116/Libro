namespace Libro.Domain.Exceptions
{
    public class UserOrRoleNotFoundException : Exception
    {
        public UserOrRoleNotFoundException()
            : base("UserOrRole Not_Found 404")
        {
        }
    }
}
