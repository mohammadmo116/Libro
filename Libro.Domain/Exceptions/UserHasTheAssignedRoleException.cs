namespace Libro.Domain.Exceptions
{
    public class UserHasTheAssignedRoleException : Exception
    {
        public UserHasTheAssignedRoleException()
          : base("The User Already Has The Assigned Role")
        {
        }
    }
}
