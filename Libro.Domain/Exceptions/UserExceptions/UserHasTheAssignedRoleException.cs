namespace Libro.Domain.Exceptions.UserExceptions
{
    public class UserHasTheAssignedRoleException : Exception
    {
        public UserHasTheAssignedRoleException()
          : base("The User Already Has The Assigned Role")
        {
        }
    }
}
